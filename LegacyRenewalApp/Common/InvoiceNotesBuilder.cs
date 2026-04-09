using System.Collections.Generic;

namespace LegacyRenewalApp.Common
{
    public class InvoiceNotesBuilder
    {
        private readonly List<string> _notes = new List<string>();

        public void Add(string note)
        {
            if (!string.IsNullOrWhiteSpace(note))
            {
                _notes.Add(note.Trim());
            }
        }

        public void AddRange(IEnumerable<string> notes)
        {
            if (notes == null)
            {
                return;
            }

            foreach (var note in notes)
            {
                Add(note);
            }
        }

        public string Build()
        {
            return string.Join("; ", _notes);
        }
    }
}