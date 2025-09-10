using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace JType
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load system fonts
            foreach (FontFamily font in Fonts.SystemFontFamilies)
                FontFamilyBox.Items.Add(font.Source);

            // Load font sizes
            for (int i = 8; i <= 48; i += 2)
                FontSizeBox.Items.Add(i);

            FontFamilyBox.SelectedItem = "Segoe UI";
            FontSizeBox.SelectedItem = 14;
        }

        // Get current RichTextBox
        private RichTextBox? GetCurrentEditor()
        {
            if (TabEditor.SelectedContent is RichTextBox editor)
                return editor;
            if (TabEditor.SelectedContent is Panel panel && panel.Children[0] is RichTextBox rtb)
                return rtb;
            return null;
        }

        // --- File Operations ---
        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = new TabItem { Header = $"Document {TabEditor.Items.Count + 1}" };
            RichTextBox editor = new RichTextBox
            {
                AcceptsTab = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            SpellCheck.SetIsEnabled(editor, true);

            tab.Content = editor;
            TabEditor.Items.Add(tab);
            TabEditor.SelectedItem = tab;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor == null) return;

            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                using FileStream fs = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(editor.Document.ContentStart, editor.Document.ContentEnd);

                if (Path.GetExtension(dlg.FileName).ToLower() == ".rtf")
                    range.Load(fs, DataFormats.Rtf);
                else
                    range.Load(fs, DataFormats.Text);

                ((TabItem)TabEditor.SelectedItem).Header = Path.GetFileName(dlg.FileName);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor == null) return;

            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                using FileStream fs = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(editor.Document.ContentStart, editor.Document.ContentEnd);

                if (Path.GetExtension(dlg.FileName).ToLower() == ".rtf")
                    range.Save(fs, DataFormats.Rtf);
                else
                    range.Save(fs, DataFormats.Text);

                ((TabItem)TabEditor.SelectedItem).Header = Path.GetFileName(dlg.FileName);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        // --- Text Formatting ---
        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor != null)
                EditingCommands.ToggleBold.Execute(null, editor);
        }

        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor != null)
                EditingCommands.ToggleItalic.Execute(null, editor);
        }

        private void Underline_Click(object sender, RoutedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor != null)
                EditingCommands.ToggleUnderline.Execute(null, editor);
        }

        private void FontFamilyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor == null) return;

            if (FontFamilyBox.SelectedItem != null)
                editor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty,
                                                    new FontFamily(FontFamilyBox.SelectedItem.ToString()));
        }

        private void FontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor == null) return;

            if (FontSizeBox.SelectedItem != null)
                editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty,
                                                    (double)(int)FontSizeBox.SelectedItem);
        }

        private void TextColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            var editor = GetCurrentEditor();
            if (editor == null || !e.NewValue.HasValue) return;

            editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty,
                                                new SolidColorBrush(e.NewValue.Value));
        }

        private void HighlightColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            var editor = GetCurrentEditor();
            if (editor == null || !e.NewValue.HasValue) return;

            editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty,
                                                new SolidColorBrush(e.NewValue.Value));
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor != null && editor.CanUndo)
                editor.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            var editor = GetCurrentEditor();
            if (editor != null && editor.CanRedo)
                editor.Redo();
        }
    }
}