// This script creates the full spec doc with freshly numbered section headers, adjusted reference links and ToC.
// Note that the reference links do work on github and in VS Code, but not with all other markdown dialects. For
// releases (which will probably be html or pdf), a conversion tool must be used that preserves the links, or else
// this build script must be updated to add proper name tags to the headings.

// Configuration of file locations and some document elements
let specDir = "spec"
let outDir = "artifacts"
let outName = "spec"
let specPath filename = $"{specDir}/{filename}"
let chapterPath chapterName = specPath chapterName + ".md"
let catalogPath = specPath "Catalog.json"
let outFilePath = $"{outDir}/{outName}.md"
let title = ["The F# Language Specification"; "============================="; ""]
let versionPlaceholder () = [$"_This is an inofficial version, created from sources on {System.DateTime.Now}_"; ""]
let tocHeader = [""; "# Table of Contents"]

open System
open System.Text.RegularExpressions
open System.Text.Json
open System.IO

type Chapter = {name: string; lines: string list}
type Chapters = {frontMatter: Chapter; clauses: Chapter list}
type Catalog = {FrontMatter: string; MainBody: string list; Annexes: string list}
type BuildState = {
    chapterName: string
    lineNumber: int
    sectionNumber: int list
    inCodeBlock: bool
    toc: Map<int list, string>
    errors: string list
}
type BuildError =
    | IoFailure of string
    | DocumentErrors of string list

let initialState = {
    chapterName = ""
    lineNumber = 0
    sectionNumber = []
    inCodeBlock = false
    toc = Map.empty
    errors = []
}

let readChapters () =
    try
        use catalogStream = File.OpenRead catalogPath
        let catalog = JsonSerializer.Deserialize<Catalog> catalogStream
        let getChapter name = {name = name; lines = File.ReadAllLines(chapterPath name) |> Array.toList}
        let clauses = catalog.MainBody |> List.map getChapter
        let frontMatter = getChapter catalog.FrontMatter
        let totalChapters = clauses.Length + 1
        let totalLines = List.sumBy (_.lines >> List.length) clauses + frontMatter.lines.Length
        printfn $"read {totalChapters} files with a total of {totalLines} lines"
        Ok {frontMatter = frontMatter; clauses = clauses}
    with e ->
        Error(IoFailure e.Message)

let writeSpec (lines: string list) =
    try
        if not <| Directory.Exists outDir then
            Directory.CreateDirectory outDir |> ignore
        File.WriteAllLines(outFilePath, lines)
        printfn $"created {outFilePath}"
        Ok()
    with e ->
        Error(IoFailure e.Message)

let sectionText sectionNumber =
    $"""{List.head sectionNumber}.{List.tail sectionNumber |> List.map string |> String.concat "."}"""

let newSection level prevSection =
    let rec newSectionR prevSectionR =
        match prevSectionR with
        | [] -> [1]
        | h :: t when prevSectionR.Length = level -> (h + 1) :: t
        | _ :: t when prevSectionR.Length > level -> newSectionR t
        | _ when prevSectionR.Length = level - 1 -> 1 :: prevSectionR
        | _ -> []
    newSectionR (List.rev prevSection) |> List.rev

let kebabCase (s: string) =
    let convertChar c =
        if Char.IsAsciiLetterLower c || c = '-' || Char.IsAsciiDigit c then Some c
        elif Char.IsAsciiLetterUpper c then Some(Char.ToLower c)
        elif c = ' ' then Some '-'
        else None
    s |> Seq.choose convertChar |> Seq.toArray |> String

let mkError state msg = $"{state.chapterName}.md({state.lineNumber}): {msg}"

let checkCodeBlock state line =
    let m = Regex.Match(line, " *```(.*)")
    if not m.Success then
        state
    else if state.inCodeBlock then
        {state with inCodeBlock = false}
    else
        let infoString = m.Groups[1].Value
        let validInfoStrings = ["fsgrammar"; "fsharp"; "csharp"; "fsother"]
        if not <| List.contains infoString validInfoStrings then
            let validFences = validInfoStrings |> List.map ((+) "```") |> String.concat ", "
            let msg = $"starting code block fences must be one of {validFences}"
            {state with inCodeBlock = true; errors = (mkError state msg) :: state.errors}
        else
            {state with inCodeBlock = true}

let renumberIfHeaderLine state line =
    let state = {state with lineNumber = state.lineNumber + 1}
    let state = checkCodeBlock state line
    let m = Regex.Match(line, "^(#+) +(.*)")
    if state.inCodeBlock || not m.Success then
        line, state
    else
        let headerPrefix = m.Groups[1].Value
        let level = headerPrefix.Length
        let heading = m.Groups[2].Value
        let m = Regex.Match(heading, "^\d")
        if m.Success then
            let msg = "Headers must not start with digits"
            line, {state with errors = (mkError state msg) :: state.errors}
        else
            let sectionNumber = newSection level state.sectionNumber
            if sectionNumber.IsEmpty then
                let msg = $"The header level jumps from {state.sectionNumber.Length} to {level}"
                line, {state with errors = (mkError state msg) :: state.errors}
            else
                let headerLine = $"{headerPrefix} {sectionText sectionNumber} {heading}"
                headerLine, {state with sectionNumber = sectionNumber; toc = state.toc.Add(sectionNumber, heading)}

let renumberClause state clause =
    let state = {state with chapterName = clause.name; lineNumber = 0}
    let outLines, state = (state, clause.lines) ||> List.mapFold renumberIfHeaderLine
    {clause with lines = outLines}, state

let tocLines toc =
    let tocLine (number, heading) =
        let sText = sectionText number
        let anchor = $"#{kebabCase sText}-{kebabCase heading}"
        String.replicate (number.Length - 1) "  " + $"- [{sText} {heading}]({anchor})"
    toc |> Map.toList |> List.map tocLine

let adjustLinks state line =
    let state = {state with lineNumber = state.lineNumber + 1}
    let rec adjustLinks' state lineFragment =
        let m = Regex.Match(lineFragment, "(.*)\[§(\d+\.[\.\d]*)\]\(([^#)]+)#([^)]+)\)(.*)")
        if m.Success then
            let pre, sText, filename, anchor, post =
                m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value, m.Groups[5].Value
            match Map.tryPick (fun n heading -> if sectionText n = sText then Some heading else None) state.toc with
            | Some _ ->
                let post', state' = adjustLinks' state post  // recursive check for multiple links in a line
                $"{pre}[§{sText}](#{kebabCase sText}-{anchor}){post'}", state'
            | None ->
                let msg = $"unknown link target {filename}#{anchor} ({sText})"
                lineFragment, {state with errors = (mkError state msg) :: state.errors}
        else
            lineFragment, state
    adjustLinks' state line

let processChapters chapters =
    // Add section numbers to the headers, collect the ToC information, and check for correct code fence info strings
    let (processedClauses, state) = (initialState, chapters.clauses) ||> List.mapFold renumberClause
    // Create the ToC and build the complete spec
    let lines =
        List.concat [
            title
            versionPlaceholder ()
            chapters.frontMatter.lines
            tocHeader
            tocLines state.toc
            List.collect _.lines processedClauses
        ]
    // Adjust the reference links to point to the correct header of the new spec
    let (lines, state) = ({state with chapterName = outName; lineNumber = 0}, lines) ||> List.mapFold adjustLinks
    if not state.errors.IsEmpty then Error(DocumentErrors(List.rev state.errors)) else Ok lines

let build () =
    match readChapters () |> Result.bind processChapters |> Result.bind writeSpec with
    | Ok() -> 0
    | Error(IoFailure msg) ->
        printfn $"IO error: %s{msg}"
        1
    | Error(DocumentErrors errors) ->
        errors |> List.iter (printfn "Error: %s")
        2

build ()
