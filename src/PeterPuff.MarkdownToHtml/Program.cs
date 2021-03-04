using Markdig;
using Markdig.Prism;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace PeterPuff.MarkdownToHtml
{
    partial class Program
    {
        internal static int Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine($"Usage: {Path.GetFileName(Assembly.GetExecutingAssembly().Location)} <markdown-file> <html-template-file> <output-file> <embed-images>");
                Console.WriteLine("<markdown-file>: Path to Markdown file to convert");
                Console.WriteLine("<html-template-file>: HTML file to use as template");
                Console.WriteLine("<output-file>: Path to output HTML file to write");
                Console.WriteLine("<embed-images>: 1 to embed (local) images as base64; 0 to keep image references as they are");
                return 1;
            }

            string markdownFile = args[0];
            string htmlTemplateFile = args[1];
            string outputFile = args[2];
            bool embedImages = args[3] == "1";

            var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(options => options.SingleLine = true));
            var logger = loggerFactory.CreateLogger(string.Empty);

            if (File.Exists(markdownFile) == false)
            {
                logger.LogError(new EventId(2), "Markdown file '{MarkdownFile}' does not exist!", markdownFile);
                return 2;
            }

            if (File.Exists(htmlTemplateFile) == false)
            {
                logger.LogError(new EventId(3), "HTML template file '{HtmlTemplateFile}' does not exist!", htmlTemplateFile);
                return 3;
            }

            logger.LogInformation(new EventId(1001), "Converting '{MarkdownFile}' to '{OutputFile}' using '{HtmlTemplateFile}'...", markdownFile, outputFile, htmlTemplateFile);

            var sourceDirectory = new FileInfo(markdownFile).Directory.FullName;

            string markdown;
            try
            {
                markdown = File.ReadAllText(markdownFile);
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(4), "An error occured while reading contents of Markdown file '{MarkdownFile}': {ErrorMessage}", markdownFile, ex.Message, ex);
                return 4;
            }

            string htmlTemplate;
            try
            {
                htmlTemplate = File.ReadAllText(htmlTemplateFile);
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(5), "An error occured while reading contents of HTML template file '{HtmlTemplateFile}': {ErrorMessage}", htmlTemplateFile, ex.Message, ex);
                return 5;
            }

            try
            {
                ConvertMarkdownToHtml(markdown, htmlTemplate, outputFile, sourceDirectory, embedImages, logger);
                logger.LogInformation(new EventId(1002), "...success");
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(6), "An error occured while converting Markdown to HTML and writing to file '{OutputFile}': {ErrorMessage}", outputFile, ex.Message, ex);
                return 6;
            }

            return 0;
        }

        private static void ConvertMarkdownToHtml(string markdown, string htmlTemplate, string outputFile, string sourceDirectory, bool embedImages, ILogger logger)
        {
            var pipeline = CreateMarkdownPipeline(embedImages, sourceDirectory);
            var html = Markdown.ToHtml(markdown, pipeline);
            html = htmlTemplate.Replace("{$html}", html);
            File.WriteAllText(outputFile, html);
        }

        private static MarkdownPipeline CreateMarkdownPipeline(bool embedImages, string sourceDirectory)
        {
            var builder = new MarkdownPipelineBuilder();
            builder.UseAdvancedExtensions();
            builder.UsePrism();
            builder.DebugLog = Console.Out;

            if (embedImages)
                builder.Use(new EmbedLocalImagesExtension(sourceDirectory));

            return builder.Build();
        }
    }
}
