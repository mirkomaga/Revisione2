﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Revisione2
{
    class Controller
    {
        public static List<FileInfo> getFolderFile(string path)
        {
            List<FileInfo> fileInfoList = new List<FileInfo>();

            string[] allfiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            foreach (string f in allfiles)
            {
                FileInfo info = new FileInfo(f);

                fileInfoList.Add(info);
            }

            return fileInfoList;
        }
        public static List<FileInfo> getMostUpdated(List<FileInfo> list)
        {
            IDictionary<string, List<FileInfo>> tmpDict = new Dictionary<string, List<FileInfo>>();

            foreach (FileInfo fi in list)
            {
                string name = fi.Name;

                if (!tmpDict.ContainsKey(name))
                {
                    tmpDict.Add(name, new List<FileInfo>());
                }

                tmpDict[name].Add(fi);
            }

            List<FileInfo> fileRecenti = new List<FileInfo>();

            foreach (var x in tmpDict)
            {
                if (x.Value.Count > 1)
                {
                    FileInfo fUpdated = x.Value.OrderBy(y => y.LastWriteTime).ToList().Last();
                    fileRecenti.Add(fUpdated);
                }
                else
                {
                    fileRecenti.Add(x.Value.First());
                }
            }

            return fileRecenti;
            ;
        }
        public static (List<FileInfo>, IDictionary<string, bool>) trovoRevisioniRecenti(List<FileInfo> listaOld, List<FileInfo> listaNew)
        {
            List<FileInfo> fileAggiornati = new List<FileInfo>();
            IDictionary<string, bool> riferimento = new Dictionary<string, bool>();

            // ? PER OGNI FILE CON LO STESSO NOME PRENDERE IL PIU RECENTE
            listaNew = getMostUpdated(listaNew);
            listaOld = getMostUpdated(listaOld);

            List<FileInfo> results = new List<FileInfo>();

            foreach (FileInfo fInfoNew in listaNew)
            {
                FileInfo fInfoOld = listaOld.Find(x => x.Name == fInfoNew.Name);

                if (fInfoOld != null)
                {
                    if (fInfoOld.LastWriteTime < fInfoNew.LastWriteTime)
                    {
                        // ? IL FILE E' STATO AGGIORNATO
                        fileAggiornati.Add(fInfoNew);
                        riferimento.Add(fInfoNew.Name, false);
                    }
                }
                else
                {
                    // ? IL FILE E' NUOVO
                    fileAggiornati.Add(fInfoNew);
                    riferimento.Add(fInfoNew.Name, true);
                }
            }

            return (fileAggiornati, riferimento);
        }
        public static IDictionary<string, bool> replaceFileInOldFolder(List<FileInfo> fileAggiornati, string pathOld)
        {

            IDictionary<string, bool> results = new Dictionary<string, bool>();

            foreach (FileInfo fi in fileAggiornati)
            {
                try
                {
                    File.Copy(fi.FullName, pathOld + "\\" + Path.GetFileName(fi.FullName), true);
                    results.Add(Path.GetFileName(fi.FullName), true);
                }
                catch
                {
                    results.Add(Path.GetFileName(fi.FullName), false);
                }
            }

            return results;
        }
        public static void generaFolderNuoviFile(List<FileInfo> fileAggiornati)
        {
            string path = creaFolder();

            foreach (FileInfo fi in fileAggiornati)
            {
                File.Copy(fi.FullName, path + "\\" + Path.GetFileName(fi.FullName), true);
            }
            try
            {
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", @path);
            }
            catch
            {
                MessageBox.Show("File salvato in: " + path);
            }
        }
        public static string creaFolder()
        {
            try
            { 
                Directory.Delete("tmpRevisione", true);
            }
            catch{}

            try
            {
                DirectoryInfo di = Directory.CreateDirectory("tmpRevisione");
                return di.FullName;
            }
            catch { }

            return "";
        }
    }

}
