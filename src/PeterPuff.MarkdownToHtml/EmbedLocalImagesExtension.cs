using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace PeterPuff.MarkdownToHtml
{
    partial class Program
    {
        internal sealed class EmbedLocalImagesExtension : IMarkdownExtension
        {
            public ILogger Logger { get; }
            public string SourceWorkingDirectory { get; }

            public EmbedLocalImagesExtension(string sourceWorkingDirectory, ILogger logger = null)
            {
                SourceWorkingDirectory = sourceWorkingDirectory ?? throw new ArgumentNullException(nameof(sourceWorkingDirectory));
                Logger = logger;
            }

            public void Setup(MarkdownPipelineBuilder pipeline)
                => pipeline.DocumentProcessed += Pipeline_DocumentProcessed;

            public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }

            private void Pipeline_DocumentProcessed(MarkdownDocument document)
            {
                var images = document.Descendants()
                    .OfType<LinkInline>()
                    .Where(inlineLink => inlineLink.IsImage);

                foreach (var image in images)
                {
                    var url = image.Url;
                    try
                    {
                        url = Path.GetFullPath(url, SourceWorkingDirectory);
                        if (File.Exists(url) == false)
                            continue;

                        var bytes = File.ReadAllBytes(url);
                        var base64 = Convert.ToBase64String(bytes);
                        var mimeType = MimeMapping.MimeUtility.GetMimeMapping(url);
                        var newUrl = $"data:{mimeType};base64,{base64}";

                        image.Url = newUrl;
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogWarning(new EventId(101), "An error occured while embedding image with URL '{ImageUrl}'", url, ex);
                    }
                }
            }
        }
    }
}
