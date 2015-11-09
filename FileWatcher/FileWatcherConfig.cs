using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileWatcher
{
    public class FileWatcherConfig
    {
        private string sourceDir;
        private string ftpHost;
        private int ftpPort;
        private string ftpUser;
        private string ftpPass;

        public FileWatcherConfig()
        {
            sourceDir = "";
            ftpHost = "";
            ftpPort = 0;
            ftpUser = "";
            ftpPass = "";
        }

        public String sourcedir
        {
            get
            {
                return sourceDir;
            }
            set
            {
                sourceDir = value;
            }
        }

        public String host
        {
            get
            {
                return ftpHost;
            }
            set
            {
                ftpHost = value;
            }
        }

        public int port
        {
            get
            {
                return ftpPort;
            }
            set
            {
                ftpPort = value;
            }
        }

        public String username
        {
            get
            {
                return ftpUser;
            }
            set
            {
                ftpUser = value;
            }
        }

        public String password
        {
            get
            {
                return ftpPass;
            }
            set
            {
                ftpPass = value;
            }
        }

    }
}