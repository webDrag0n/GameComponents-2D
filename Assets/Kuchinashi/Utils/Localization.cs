using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Kuchinashi.Utils
{
    public class Localization
    {
        public static string Get(string key)
        {
            string lang = PlayerPrefs.GetString("Language", "en");
            string path = $"{Application.streamingAssetsPath}/Kuchinashi/I18n/{lang}/base.xml";

            if (File.Exists(path))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                if (xmlDoc.SelectSingleNode("Localization").Attributes["lang"].Value != lang)
                {
                    throw new Exception("Localization File Error.");
                }

                XmlNodeList nodeList = xmlDoc.SelectSingleNode("Localization")?.ChildNodes;

                if (nodeList != null)
                {
                    foreach (XmlElement node in nodeList)
                    {
                        if (node.Name == key)
                        {
                            return node.InnerText.Replace("\\n", Environment.NewLine);
                        }
                    }
                }

                throw new Exception("Item not found.");
            }

            throw new Exception("Localization File not found.");
        }
        
        public static string Get(string key, string lang)
        {
            string path = Application.streamingAssetsPath + $"/I18n/{lang}/base.xml";

            if (File.Exists(path))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                if (xmlDoc.SelectSingleNode("Localization").Attributes["lang"].Value != lang)
                {
                    throw new Exception("Localization File Error.");
                }

                XmlNodeList nodeList = xmlDoc.SelectSingleNode("Localization")?.ChildNodes;

                if (nodeList != null)
                {
                    foreach (XmlElement node in nodeList)
                    {
                        if (node.Name == key)
                        {
                            return node.InnerText.Replace("\\n", Environment.NewLine);
                        }
                    }
                }

                throw new Exception("Item not found.");
            }

            throw new Exception("Localization File not found.");
        }
    }
}