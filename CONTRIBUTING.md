# Contributing to this project

## Writing a specification

Writing a good spec is an art in itself. You must be very precise while using natural language, which by its nature is imprecise. Have a look at other parts of the F# spec, or better at the [C# spec](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/readme), which was created with much more effort by many more people.

## Guidelines for editing the markdown sources

The source for the spec is the collection of markdown files in the spec folder.

#### Markdown flavor

We aim to use [CommonMark](https://spec.commonmark.org/0.31.2/) markdown, plus the [section links](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#section-links) and [tables](https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/organizing-information-with-tables#creating-a-table) of github-flavored markdown.

#### Headings

We use [ATX headings](https://spec.commonmark.org/0.31.2/#atx-headings) without closing # characters.

In the sources, the headings are not numbered. Numbering is added during the build process.

#### Links

Intra-spec links are made like this: `[§](inference-procedures.md#constraint-solving)`.

As such, the links work directly in the spec sources (like on github or in VS Code preview).

They are converted during the build process to include the section number.

#### Other markdown guidelines

All [fenced code blocks](https://spec.commonmark.org/0.31.2/#fenced-code-blocks) should carry one of the following info strings.

- `fsharp` for F# code samples
- `csharp` for C# code samples
- `fsgrammar` for F# grammar
- `fsother` for other code (like pseudo code in a few places)

Show _defined terms_ in italics.

For `inline code` (including e.g. file and type names) use code spans.

## Viewing the result

In most cases, checking your edits in a markdown viewer should be sufficient. But you can also view the final output by proceeding as follows.

- Make your changes in the local clone of your forked repo in a branch called `dev`.
- Push your changes. This will automatically create a branch gh-pages in your repo.
- When you have done the above for the first time, go to github settings of your forked repo and enable github pages (Settings -> Pages -> Branch gh-pages -> Save).
- You can view the spec with your changes at `http://<yourAccountName>.github.io/fslang-spec`.



