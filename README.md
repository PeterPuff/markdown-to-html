# Markdown to HTML

[![Build + Test](https://github.com/PeterPuff/markdown-to-html/actions/workflows/build-test.yml/badge.svg)](https://github.com/PeterPuff/markdown-to-html/actions/workflows/build-test.yml)

## Description

Markdown to HTML is a tool to convert Markdown documents to HTML pages. 

It uses [Markdig](https://github.com/xoofx/markdig) to convert the document. [Markdig.Prism](https://github.com/ilich/Markdig.Prism) is used to enable syntax highlighting via [Prism.js](https://prismjs.com/).

You need a (at least basic) HTML template file, which has to be supplied when the tool is invoked.

## HTML Template

As mentioned before, this tool needs a HTML template file, which will be used to create the HTML output file.

Within the `<body>`-Tag of this template must be a occurence of `{$html}`. This occurence will be replaced with the output of Markdig.

This is an example of a basic HTML template without styling:

```html
<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta charset="utf-8" />
    </head>
    <body>
        <div>
{$html}
        </div>
    </body>
</html>
```

## Embed local images

The tool has an option to embed linked local images as base64. This is useful to create a single HTML page without further dependencies, e.g. to create end user friedly documentation in an automated release process.

## Usage

The tool can be installed as global dotnet tool via `dotnet tool install -g PeterPuff.MarkdownToHtml`.

After installing you can invoke it via: `mdtohtml <markdown-file> <html-template-file> <output-file> <embed-images> <use-prism>`.

### Arguments

- `<markdown-file>`: Path to the Markdown file to convert
- `<html-template-file>`: HTML file to use as template
- `<output-file>`: Path to output HTML file to write
- `<embed-images>`: 1 to embed (local) images as base64; 0 to keep image references as they are
- `<use-prism>`: 1 to enable syntax highlighting via Prism.js; 0 if Prism.js is not used in the template
