using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace llm_agent.Common.Utils
{
    /// <summary>
    /// 将Markdown文本转换为RichTextBox可显示的富文本格式
    /// </summary>
    public class MarkdownToRichTextConverter
    {
        private readonly RichTextBox _richTextBox;
        private readonly MarkdownPipeline _pipeline;
        private readonly Color _defaultTextColor;
        private readonly Font _defaultFont;
        private readonly Font _boldFont;
        private readonly Font _italicFont;
        private readonly Font _boldItalicFont;
        private readonly Font _headingFont;
        private readonly Font _codeFont;
        private int _startPosition;

        /// <summary>
        /// 初始化Markdown转RichText转换器
        /// </summary>
        /// <param name="richTextBox">目标RichTextBox控件</param>
        public MarkdownToRichTextConverter(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox ?? throw new ArgumentNullException(nameof(richTextBox));
            _defaultTextColor = Color.Black;
            _defaultFont = richTextBox.Font;
            _boldFont = new Font(_defaultFont, FontStyle.Bold);
            _italicFont = new Font(_defaultFont, FontStyle.Italic);
            _boldItalicFont = new Font(_defaultFont, FontStyle.Bold | FontStyle.Italic);
            _headingFont = new Font(_defaultFont.FontFamily, _defaultFont.Size * 1.2f, FontStyle.Bold);
            _codeFont = new Font("Consolas", _defaultFont.Size, FontStyle.Regular);

            // 配置Markdig解析管道
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
        }

        /// <summary>
        /// 将Markdown文本渲染到RichTextBox中
        /// </summary>
        /// <param name="markdownText">Markdown格式的文本</param>
        /// <param name="startPosition">起始位置</param>
        public void RenderMarkdown(string markdownText, int startPosition)
        {
            if (string.IsNullOrEmpty(markdownText))
                return;

            _startPosition = startPosition;
            
            try
            {
                // 解析Markdown文档
                var document = Markdown.Parse(markdownText, _pipeline);
                
                // 遍历文档中的所有块元素
                foreach (var block in document)
                {
                    RenderBlock(block);
                }
            }
            catch (Exception ex)
            {
                // 如果解析失败，以纯文本形式显示
                _richTextBox.SelectionStart = startPosition;
                _richTextBox.SelectionLength = 0;
                _richTextBox.SelectionColor = _defaultTextColor;
                _richTextBox.SelectionFont = _defaultFont;
                _richTextBox.AppendText(markdownText);
                
                Console.WriteLine($"Markdown解析错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 检测文本是否包含Markdown格式
        /// </summary>
        /// <param name="text">要检查的文本</param>
        /// <returns>是否包含Markdown格式</returns>
        public static bool ContainsMarkdown(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            // 检查常见的Markdown语法
            var patterns = new[]
            {
                @"#{1,6}\s+.*", // 标题
                @"\*\*.+?\*\*", // 粗体
                @"\*.+?\*", // 斜体
                @"~~.+?~~", // 删除线
                @"`[^`]+`", // 行内代码
                @"```[\s\S]*?```", // 代码块
                @"\[[^\]]+\]\([^\)]+\)", // 链接
                @"!\[[^\]]*\]\([^)]+\)", // 图片
                @"^\s*[\*\-\+]\s+.+", // 无序列表
                @"^\s*\d+\.\s+.+", // 有序列表
                @">\s+.+" // 引用
            };

            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(text, pattern, RegexOptions.Multiline))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 渲染Markdown块
        /// </summary>
        private void RenderBlock(Block block)
        {
            switch (block)
            {
                case HeadingBlock heading:
                    RenderHeading(heading);
                    break;
                case ParagraphBlock paragraph:
                    RenderParagraph(paragraph);
                    break;
                case QuoteBlock quote:
                    RenderQuote(quote);
                    break;
                case CodeBlock code:
                    RenderCodeBlock(code);
                    break;
                case ListBlock list:
                    RenderList(list);
                    break;
                case ThematicBreakBlock _:
                    RenderThematicBreak();
                    break;
                default:
                    // 处理其他块类型或嵌套块
                    if (block is ContainerBlock container)
                    {
                        foreach (var childBlock in container)
                        {
                            RenderBlock(childBlock);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 渲染标题
        /// </summary>
        private void RenderHeading(HeadingBlock heading)
        {
            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            
            // 根据标题级别调整字体大小
            float size = _defaultFont.Size;
            switch (heading.Level)
            {
                case 1:
                    size *= 1.6f;
                    break;
                case 2:
                    size *= 1.4f;
                    break;
                case 3:
                    size *= 1.2f;
                    break;
                default:
                    size *= 1.1f;
                    break;
            }
            
            var headingFont = new Font(_defaultFont.FontFamily, size, FontStyle.Bold);
            _richTextBox.SelectionFont = headingFont;
            _richTextBox.SelectionColor = Color.FromArgb(50, 50, 50);
            
            RenderInlines(heading.Inline);
            _richTextBox.AppendText(Environment.NewLine);
        }

        /// <summary>
        /// 渲染段落
        /// </summary>
        private void RenderParagraph(ParagraphBlock paragraph)
        {
            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            _richTextBox.SelectionFont = _defaultFont;
            _richTextBox.SelectionColor = _defaultTextColor;
            
            RenderInlines(paragraph.Inline);
            _richTextBox.AppendText(Environment.NewLine);
        }

        /// <summary>
        /// 渲染引用
        /// </summary>
        private void RenderQuote(QuoteBlock quote)
        {
            int quoteStart = _richTextBox.TextLength;
            
            foreach (var block in quote)
            {
                RenderBlock(block);
            }
            
            // 为引用添加左边界
            int quoteEnd = _richTextBox.TextLength;
            _richTextBox.SelectionStart = quoteStart;
            _richTextBox.SelectionLength = quoteEnd - quoteStart;
            _richTextBox.SelectionColor = Color.FromArgb(100, 100, 100);
            _richTextBox.SelectionBackColor = Color.FromArgb(245, 245, 245);
            
            // 添加引用左边线（通过绘制字符实现）
            _richTextBox.AppendText(Environment.NewLine);
        }

        /// <summary>
        /// 渲染代码块
        /// </summary>
        private void RenderCodeBlock(CodeBlock codeBlock)
        {
            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            _richTextBox.SelectionFont = _codeFont;
            _richTextBox.SelectionColor = Color.FromArgb(80, 80, 80);
            _richTextBox.SelectionBackColor = Color.FromArgb(245, 245, 245);
            
            string code = "";
            if (codeBlock is FencedCodeBlock fencedCodeBlock)
            {
                var lines = fencedCodeBlock.Lines.Lines;
                foreach (var line in lines)
                {
                    code += line.ToString() + Environment.NewLine;
                }
            }
            else
            {
                var lines = ((CodeBlock)codeBlock).Lines.Lines;
                foreach (var line in lines)
                {
                    code += line.ToString() + Environment.NewLine;
                }
            }
            
            _richTextBox.AppendText(code);
            _richTextBox.SelectionBackColor = Color.Transparent;
            _richTextBox.AppendText(Environment.NewLine);
        }

        /// <summary>
        /// 渲染列表
        /// </summary>
        private void RenderList(ListBlock list)
        {
            int index = 1;
            // 添加4个空格的缩进
            string indent = "    ";
            
            foreach (var item in list)
            {
                if (item is ListItemBlock listItem)
                {
                    _richTextBox.SelectionStart = _richTextBox.TextLength;
                    _richTextBox.SelectionLength = 0;
                    _richTextBox.SelectionFont = _defaultFont;
                    _richTextBox.SelectionColor = _defaultTextColor;
                    
                    // 添加缩进
                    _richTextBox.AppendText(indent);
                    
                    // 添加列表标记
                    if (list.IsOrdered)
                    {
                        _richTextBox.AppendText($"{index++}. ");
                    }
                    else
                    {
                        _richTextBox.AppendText("• ");
                    }
                    
                    foreach (var block in listItem)
                    {
                        RenderBlock(block);
                    }
                }
            }
            
            _richTextBox.AppendText(Environment.NewLine);
        }

        /// <summary>
        /// 渲染分隔线
        /// </summary>
        private void RenderThematicBreak()
        {
            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            
            string line = "────────────────────────────────────";
            _richTextBox.SelectionColor = Color.Gray;
            _richTextBox.AppendText(line + Environment.NewLine);
        }

        /// <summary>
        /// 渲染行内元素
        /// </summary>
        private void RenderInlines(ContainerInline inlines)
        {
            foreach (var inline in inlines)
            {
                switch (inline)
                {
                    case LiteralInline literal:
                        _richTextBox.AppendText(literal.Content.ToString());
                        break;
                    
                    case EmphasisInline emphasis:
                        RenderEmphasis(emphasis);
                        break;
                    
                    case LinkInline link:
                        RenderLink(link);
                        break;
                    
                    case CodeInline code:
                        RenderInlineCode(code);
                        break;
                    
                    case LineBreakInline:
                        _richTextBox.AppendText(Environment.NewLine);
                        break;
                    
                    default:
                        // 处理其他内联元素
                        if (inline is ContainerInline container)
                        {
                            RenderInlines(container);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 渲染强调（粗体、斜体）
        /// </summary>
        private void RenderEmphasis(EmphasisInline emphasis)
        {
            Font currentFont;
            
            // 根据强调级别选择字体样式
            if (emphasis.DelimiterCount == 2)
            {
                // 粗体
                currentFont = _boldFont;
            }
            else if (emphasis.DelimiterCount == 1)
            {
                // 斜体
                currentFont = _italicFont;
            }
            else
            {
                // 加粗斜体
                currentFont = _boldItalicFont;
            }
            
            var prevFont = _richTextBox.SelectionFont;
            _richTextBox.SelectionFont = currentFont;
            
            RenderInlines(emphasis);
            
            _richTextBox.SelectionFont = prevFont;
        }

        /// <summary>
        /// 渲染链接
        /// </summary>
        private void RenderLink(LinkInline link)
        {
            int linkStart = _richTextBox.TextLength;
            
            // 先渲染链接文本
            RenderInlines(link);
            
            // 设置链接样式
            int linkEnd = _richTextBox.TextLength;
            _richTextBox.SelectionStart = linkStart;
            _richTextBox.SelectionLength = linkEnd - linkStart;
            _richTextBox.SelectionColor = Color.Blue;
            _richTextBox.SelectionFont = new Font(_defaultFont, FontStyle.Underline);
            
            // 恢复选择位置到文档末尾
            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            _richTextBox.SelectionColor = _defaultTextColor;
            _richTextBox.SelectionFont = _defaultFont;
        }

        /// <summary>
        /// 渲染行内代码
        /// </summary>
        private void RenderInlineCode(CodeInline code)
        {
            var prevFont = _richTextBox.SelectionFont;
            var prevColor = _richTextBox.SelectionColor;
            var prevBackColor = _richTextBox.SelectionBackColor;
            
            _richTextBox.SelectionFont = _codeFont;
            _richTextBox.SelectionColor = Color.FromArgb(80, 80, 80);
            _richTextBox.SelectionBackColor = Color.FromArgb(240, 240, 240);
            
            _richTextBox.AppendText(code.Content.ToString());
            
            _richTextBox.SelectionFont = prevFont;
            _richTextBox.SelectionColor = prevColor;
            _richTextBox.SelectionBackColor = prevBackColor;
        }
    }
} 