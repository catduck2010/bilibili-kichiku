using System;
using System.Windows.Forms;
using AVManage;
using System.IO;


namespace KichikuBili
{
    public partial class Danmaku : Form
    {
        String skippedstr = ResourceCulture.GetString("Skipstr");
        String finishedstr = ResourceCulture.GetString("Handlestr");
        String finishedallstr = ResourceCulture.GetString("Finishstr");
        String copyingstr = ResourceCulture.GetString("Copyingstr");
        String foundstr = ResourceCulture.GetString("Foundstr");
        String choosestr = ResourceCulture.GetString("Selectstr");
        String failedtoopen = ResourceCulture.GetString("Failtoopenstr");
        String emptypatherror = ResourceCulture.GetString("Emptypathstr");
        String inputpathnotexisterror = ResourceCulture.GetString("Noexistpathstr");
        String filestructureerror = ResourceCulture.GetString("Structstr");
        String ifcreatepath = ResourceCulture.GetString("Currpathstr");
        String outputpathtitle = ResourceCulture.GetString("Optpath");
        public Danmaku()
        {
            InitializeComponent();
        }
        public void CopyAndRename(String outputPath, FileSystemInfo info, int count)
        {
            if (!info.Exists) return;
            DirectoryInfo dir = info as DirectoryInfo;
            //不是目录   
            if (dir == null) return;
            DirectoryInfo[] dirs = dir.GetDirectories("*");
            double progress = (double)count;
            for (int i = 0; i < dirs.Length; i++)
            {//文件夹 D:\\tv.danmaku.bili\\下的各个文件夹
                DirectoryInfo[] dirpage = dirs[i].GetDirectories("*");
                SingleAVProcess(dirpage, outputPath);
                progressBar1.Value = (int)((i + 1) * 100 / progress);
            }
            LogOutput(finishedallstr);
        }
        public void SingleAVProcess(DirectoryInfo[] dirpage, string outputPath)
        {
            bool find = false;
            //bool nameisLegal = true;
            string videoname = "";
            string videoalias = "";
            string videoid = "";
            for (int j = 0; j < dirpage.Length; j++)
            {//D:\\tv.danmaku.bili\\21117309\\下的各个集数
                FileSystemInfo[] page = dirpage[j].GetFileSystemInfos("*");
                string videodir = "";
                string videoepi = "";
                for (int ii = 0; ii < page.Length; ii++)
                {
                    FileInfo file = page[ii] as FileInfo;
                    string timestr = "";
                    if (file != null && file.Name.Equals("entry.json"))
                    {
                        string context = "";
                        //json文件转字符串
                        {
                            try
                            {
                                context = Tools.FileToString(file.FullName);
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        //未获取AV号
                        if (videoid.Equals(""))
                        {
                            videoname = Tools.AVInfoGetter(context, "title");
                            videoalias = videoname;
                            videoid = Tools.AVInfoGetter(context, "avid");

                            bool cancel = false;
                            //AV名合法性
                            if (!Tools.NameCheck(videoalias))
                            {
                                NameConflict ncf = new NameConflict(videoalias);
                                ncf.ShowDialog();
                                videoalias = ncf.alias;
                                cancel = ncf.cancel;
                            }
                            timestr = DateTime.Now.ToString("HH:mm:ss");
                            if (cancel)
                            {
                                LogOutput($"{skippedstr} AV{videoid}");
                                //OputTextBox.Text = $"{OputTextBox.Text}{timestr} 已跳过AV{videoid}\r\n";
                                Console.WriteLine("{0} {1} AV{2}", timestr, skippedstr, videoid);
                                return;
                            }
                        }
                        videodir = Tools.AVInfoGetter(context, "type_tag");
                        videoepi = Tools.AVInfoGetter(context, "part");
                        //bool cancel = false;
                        if (!Tools.NameCheck(videoepi))
                        {
                            NameConflict ncf = new NameConflict(videoepi, 0);
                            videoepi = ncf.alias;
                            if (ncf.cancel)
                            {
                                LogOutput($"{skippedstr} P{j + 1} AV{videoid}");
                                Console.WriteLine("{0} {1}AV{2}", timestr, skippedstr, videoid);
                                break;
                            }
                        }
                        //timestr = DateTime.Now.ToString("HH:mm:ss");
                        //OputTextBox.Text = $"{OputTextBox.Text}{timestr} 找到AV{videoid}...";
                        Console.WriteLine("{0} {1}AV{2}-{3}-P{4}", timestr, foundstr,videoid, videoname, ii + 1);
                        find = true; break;
                    }
                }
                if (find)
                {
                    find = false;
                    DirectoryInfo[] lastpage = dirpage[j].GetDirectories(videodir);
                    for (int x = 0; x < lastpage.Length; x++)
                    {
                        FileSystemInfo[] content = lastpage[x].GetFileSystemInfos();
                        for (int y = 0; y < content.Length; y++)
                        {
                            FileInfo ff = content[y] as FileInfo;
                            if (ff != null && ff.Name.Equals("0.blv"))
                            {
                                bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                                File.Copy(ff.FullName, $"{outputPath}\\AV{videoid}_{videoalias}_{videoepi}.flv", isrewrite);
                                LogOutput($"{copyingstr} P{j + 1} AV{videoid}");
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"{finishedstr}AV{videoid} - {videoname}");
            //OputTextBox.Text = $"{OputTextBox.Text}完成！\r\n";
        }
        public void LogOutput(string log)
        {
            string timestr = DateTime.Now.ToString("HH:mm:ss");
            OputTextBox.AppendText(timestr + "  " + log + "\r\n");
            //OputTextBox.w
        }
        public void Browse(String rootPath, String outputPath, int count)
        {
            try
            {
                CopyAndRename(outputPath, new DirectoryInfo(rootPath), Tools.CountFolder(rootPath));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                OputTextBox.AppendText($"{e.Message}\r\n");
            }
        }
        private void BrowseButton1_Click(object sender, EventArgs e)
        {
            String path = "";
            FolderBrowserDialog fodialog = null;
            try
            {
                fodialog = new FolderBrowserDialog();
                fodialog.Description = choosestr;
                //opfiledia = new OpenFileDialog();
                //opfiledia.
                if (fodialog.ShowDialog() == DialogResult.OK)
                {
                    path = fodialog.SelectedPath;
                    IputpathBox.Text = path;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(failedtoopen + ex.Message.ToString());
            }
        }

        private void BrowseButton2_Click(object sender, EventArgs e)
        {
            String path = "";
            FolderBrowserDialog fodialog = null;
            try
            {
                fodialog = new FolderBrowserDialog();
                fodialog.Description = choosestr;
                //opfiledia = new OpenFileDialog();
                //opfiledia.
                if (fodialog.ShowDialog() == DialogResult.OK)
                {
                    path = fodialog.SelectedPath;
                    OputpathBox.Text = path;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(failedtoopen + ex.Message.ToString());
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            int length = 0;
            if (IputpathBox.Text.Equals("") || OputpathBox.Text.Equals(""))
            {
                MessageBox.Show(emptypatherror);
                return;
            }
            else
            {
                if (!Directory.Exists(IputpathBox.Text))
                {
                    MessageBox.Show(inputpathnotexisterror);
                    return;
                }
                length = Tools.CountFolder(IputpathBox.Text);
                if (length == 0)
                {
                    MessageBox.Show(filestructureerror);
                    return;
                }
                if (!Directory.Exists(OputpathBox.Text))
                {
                    MessageBoxButtons messButtons = MessageBoxButtons.OKCancel;
                    DialogResult dr = MessageBox.Show($"{ifcreatepath}{OputpathBox.Text}?", 
                        outputpathtitle, messButtons);
                    if (dr.Equals(DialogResult.OK))
                    {
                        Directory.CreateDirectory(OputpathBox.Text);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            OputTextBox.Text = "";
            Browse(IputpathBox.Text, OputpathBox.Text, length);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogOutput("Copyright 2018 Juliphang. All rights reserved.");
        }
    }
}
