using System.Collections.Generic;
using System.Linq;

namespace FileExplorer.Models
{
    public class FolderHistoryModel
    {
        private readonly List<string> history;
        private int position;

        public FolderHistoryModel(string startPath)
        {
            history = new List<string>();
            position = 0;
            history.Add(startPath);
        }

        public string PreviousFolder()
        {
            if (position > 0) position--; 
            return history.ElementAt(position);
        }

        public string NextFolder()
        {
            if (position < history.Count - 1) position++;
            return history.ElementAt(position);
        }

        public void Add(string path)
        {
            if (path != history.ElementAt(position))
            {
                while (position < history.Count - 1)
                {
                    history.RemoveRange(position + 1, history.Count - position - 1);
                }

                history.Add(path);
                position++;
            }
        }

        public string CurrentFolder() => history.ElementAt(position);
    }
}
