// This script creates the input for mkdocs in the artifacts directory.
// This includes copies of the markdown sources with added section headers and adjusted reference links.

open System
open System.Text.RegularExpressions
open System.Text.Json
open System.IO

// Configuration of file locations
let sourceDir = Path.Join("..", "spec")
let catalogPath = Path.Join(sourceDir,"Catalog.json")
let outDir = Path.Join("..", "artifacts")
let mkdocsDir = Path.Join(outDir, "mkdocs")
let mkdocsDocsDir = Path.Join(mkdocsDir, "docs")
let assetsDir = "assets"
let mkdocsConfigFilePath = Path.Join(mkdocsDir, "mkdocs.yml")
let mkdocsIconFilePath = Path.Join(mkdocsDocsDir, "fsharp128.png")
let mkdocsConfigSourcePath = Path.Join(assetsDir, "mkdocs.yml")
let mkdocsIconSourcePath = Path.Join(assetsDir, "fsharp128.png")

let versionPlaceholder () = [""; $"_This version was created from sources on {System.DateTime.Now}_"; ""]
let chapterFileName chapterName = $"{chapterName}.md"

type Catalog = {FrontMatter: string; RfcStatus: string; MainBody: string list}
type Chapter = {name: string; lines: string list}
type Sources = {frontMatter: Chapter; rfcStatus: Chapter; mainChapters: Chapter list}

type BuildState = {
    chapterName: string
    lineNumber: int
    sectionNumber: int list
    inCodeBlock: bool
    toc: Map<string * string, int list>
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

let readSources () =
    try
        use catalogStream = File.OpenRead catalogPath
        let catalog = JsonSerializer.Deserialize<Catalog> catalogStream
        let getChapter name = {name = name; lines = File.ReadAllLines($"{sourceDir}/{name}.md") |> Array.toList}
        let clauses = catalog.MainBody |> List.map getChapter
        let frontMatter = getChapter catalog.FrontMatter
        let rfcStatus = getChapter catalog.RfcStatus
        let totalChapters = clauses.Length + 1
        let totalLines = List.sumBy (_.lines >> List.length) clauses + frontMatter.lines.Length
        printfn $"read {totalChapters} files with a total of {totalLines} lines"
        Ok {frontMatter = frontMatter; rfcStatus = rfcStatus; mainChapters = clauses}
    with e ->
        Error(IoFailure e.Message)

let writeArtifacts chapters =
    try
        Directory.CreateDirectory outDir |> ignore
        Directory.CreateDirectory mkdocsDir |> ignore
        Directory.CreateDirectory mkdocsDocsDir |> ignore
        File.Delete mkdocsIconFilePath
        File.Copy(mkdocsIconSourcePath, mkdocsIconFilePath)
        let configLines = File.ReadAllLines mkdocsConfigSourcePath |> Array.toList
        let navLines = chapters |> List.map (fun c -> $"- {chapterFileName c.name}")
        File.WriteAllLines(mkdocsConfigFilePath, configLines @ navLines)
        for chapter in chapters do
            let chapterPath = Path.Join(mkdocsDocsDir, chapterFileName chapter.name)
            File.WriteAllLines(chapterPath, chapter.lines)
        printfn $"created {List.length chapters} chapters in {mkdocsDocsDir}"
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

let preprocessLine state line =
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
                let toc = state.toc.Add((chapterFileName state.chapterName, kebabCase heading), sectionNumber)
                headerLine, {state with sectionNumber = sectionNumber; toc = toc}

let preprocessChapter state chapter =
    let state = {state with chapterName = chapter.name; lineNumber = 0}
    let outLines, state = (state, chapter.lines) ||> List.mapFold preprocessLine
    {chapter with lines = outLines}, state

let adjustLinks state line =
    let state = {state with lineNumber = state.lineNumber + 1}
    let rec adjustLinks' state lineFragment =
        let m = Regex.Match(lineFragment, "(.*)\[§[^]]*\]\(([^#)]+)#([^)]+)\)(.*)")
        if m.Success then
            let pre, chapterFileName, anchor, post =
                m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value
            match Map.tryPick (fun a s ->
                if a = (chapterFileName, anchor) then Some (sectionText s) else None) state.toc with
            | Some sText ->
                let pre', state' = adjustLinks' state pre  // recursive check for multiple links in a line
                let adjustedLine = $"{pre'}[§{sText}]({chapterFileName}#{kebabCase sText}-{anchor}){post}"
                adjustedLine, state'
            | None ->
                let msg = $"unknown link target {anchor}"
                lineFragment, {state with errors = mkError state msg :: state.errors}
        else
            lineFragment, state
    adjustLinks' state line

let adjustChapterLinks state chapter =
    let state = {state with chapterName = chapter.name; lineNumber = 0}
    let adjustedLines, state = (state, chapter.lines) ||> List.mapFold adjustLinks
    {chapter with lines = adjustedLines}, state

let processSources chapters =
    // Add section numbers to the headers, collect the ToC information, and check for correct code fence info strings
    let preProcessedChapters, state = (initialState, chapters.mainChapters) ||> List.mapFold preprocessChapter
    
    // Adjust the reference links to point to the correct header of the new spec
    let adjustedChapters, state = (state, preProcessedChapters) ||> List.mapFold adjustChapterLinks

    let frontMatterChapter = {name = "index"; lines = chapters.frontMatter.lines @ versionPlaceholder()}
    let outputChapters = frontMatterChapter :: chapters.rfcStatus :: adjustedChapters
    
    if not state.errors.IsEmpty then Error(DocumentErrors(List.rev state.errors)) else Ok outputChapters

let build () =
    match readSources () |> Result.bind processSources |> Result.bind writeArtifacts with
    | Ok() -> 0
    | Error(IoFailure msg) ->
        printfn $"IO error: %s{msg}"
        1
    | Error(DocumentErrors errors) ->
        errors |> List.iter (printfn "Error: %s")
        2

build ()
