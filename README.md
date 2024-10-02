# fslang-spec
F# Language Specification

## Overview

This is an initiative to create a more complete and community-maintainable F# spec.

This will be no small task, but we believe it is worthwhile and we count on community contributions.

We foresee three phases:
1. Convert the [latest official spec](https://fsharp.org/specs/language-spec/4.1/FSharpSpec-4.1-latest.pdf) to markdown and create the structure and tools to make it community-maintainable. This is done.
2. Add the post-4.1 features as documented in the [RFCs](https://github.com/fsharp/fslang-design/) to the spec. Our goal: a complete F# 10 spec.
3. Make spec update part of new feature development so that an up-to-date spec can be released with every new major compiler release.

## Process

The spec is in the end closely coupled to the language design and therefore needs a) strong community contributions and b) a clearly defined final responsibility, which will be similar to the one of the [language design process](https://github.com/fsharp/fslang-design?tab=readme-ov-file#who-is-in-charge).

We foresee the following types of contributions:

- Issues and/or PRs for bug fixes.
- PRs for integration of an accepted and implemented RFC.
- Issues for proposing and discussing smaller or larger improvements to the spec
- PRs for such improvements, once the discussion converges and/or is decided by the team in charge

All PRs need to be accepted by two team reviewers for merging.

## Repository structure

The sources are the markdown files for the chapters (clauses) in the `spec` directory.
Run `build` to create a new complete spec (including ToC and updated reference links) in your `artifacts` directory.

At certain points, releases are created and published in the `releases` directory.

