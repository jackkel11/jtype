using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace JType
{
    public partial class SearchReplaceWindow : Window
    {
        private readonly RichTextBox _editor;
        private TextPointer _searchStart;

        public SearchReplaceWindow(RichTextBox editor)
        {
            InitializeComponent();
            _editor = editor;
            _searchStart = _editor.Document.ContentStart;
        }

        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            string textToFind = FindTextBox.Text;
            if (string.IsNullOrEmpty(textToFind)) return;

            TextRange? found = FindTextInRichTextBox(_searchStart, _editor.Document.ContentEnd, textToFind);
            if (found != null)
            {
                _editor.Selection.Select(found.Start, found.End);
                _editor.Focus();
                _searchStart = found.End;
            }
            else
            {
                MessageBox.Show("Text not found");
                _searchStart = _editor.Document.ContentStart;
            }
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            if (!_editor.Selection.IsEmpty)
            {
                _editor.Selection.Text = ReplaceTextBox.Text;
            }
            FindNext_Click(sender, e);
        }

        private void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            TextRange documentRange = new TextRange(_editor.Document.ContentStart, _editor.Document.ContentEnd);
            documentRange.Text = documentRange.Text.Replace(FindTextBox.Text, ReplaceTextBox.Text);
        }

        private TextRange? FindTextInRichTextBox(TextPointer start, TextPointer end, string text)
        {
            TextPointer pointer = start;

            while (pointer != null && pointer.CompareTo(end) < 0)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string runText = pointer.GetTextInRun(LogicalDirection.Forward);
                    int index = runText.IndexOf(text);
                    if (index >= 0)
                    {
                        TextPointer startPos = pointer.GetPositionAtOffset(index);
                        TextPointer endPos = startPos.GetPositionAtOffset(text.Length);
                        return new TextRange(startPos, endPos);
                    }
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            return null;
        }
    }
}