using System;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace AVManage {
    class Tools {
        public static string FileToString(String filePath) {
            string str = "";

            //获取文件内容  
            if (System.IO.File.Exists(filePath)) {
                System.IO.StreamReader file1 = new System.IO.StreamReader(filePath);//读取文件中的数据  
                str = file1.ReadToEnd();                                            //读取文件中的全部数据  

                file1.Close();
                file1.Dispose();
            }
            return str;
        }
        public static string AVInfoGetter(String str, String mode) {
            JsonReader reader = new JsonTextReader(new StringReader(str));
            string result = "";
            while (reader.Read()) {
                if (reader.Value != null) {
                    //Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    if ($"{reader.Value}".Equals(mode)) {
                        reader.Read();
                        result = $"{reader.Value}";
                    }
                } //else {
                  //Console.WriteLine("Token: {0}", reader.TokenType);
                  // }
            }
            return result;
        }
        public static bool NameCheck(object k) {
            Regex regex = new Regex(@"^([a-zA-Z]:\\)?[^\/\:\*\?\""\<\>\|\,]*$");//文件名合法性
            Match m = regex.Match($"{k}");
            if (!m.Success) return false;
            else return true;
        }
        public static int CountFolder(string path)//传入参数是文件夹路径
        {
            int count = 0;
            Regex regex = new Regex(@"^([1-9][0-9]*)$");//文件夹必须是整数AV号
            if (Directory.Exists(path)) {
                //所有子文件夹的文件夹名
                string[] dirs = Directory.GetDirectories(path);
                for (int i = 0; i < dirs.Length; i++) {
                    dirs[i] = Path.GetFileNameWithoutExtension(dirs[i]);
                    Match m = regex.Match(dirs[i]);
                    if (!m.Success) {
                        return 0;
                    }
                    count++;
                }

            }
            return count;
        }
        
    }
    
}
