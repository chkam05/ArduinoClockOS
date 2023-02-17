using ArudinoConnect.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArudinoConnect.Themes.TemplateSelectors
{
    public class PianoNoteDataTemplateSelector : DataTemplateSelector
    {

        //  VARIABLES

        public DataTemplate PianoNoteBreakDataTemplate { get; set; }

        public DataTemplate PianoNoteDataTemplate { get; set; }


        //  METHODS

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            PianoNote pianoNote = item as PianoNote;

            if (element != null && item != null)
            {
                return pianoNote.IsBreak
                    ? PianoNoteBreakDataTemplate
                    : PianoNoteDataTemplate;
            }

            return PianoNoteDataTemplate;
        }

    }
}
