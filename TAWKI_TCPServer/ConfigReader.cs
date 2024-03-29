﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAWKI_TCPServer.Interfaces;

namespace TAWKI_TCPServer
{
    public class ConfigReader : IConfigReader
    {
        private string _configPath; //test
        private string _MySQLDBConnect;
        private string _RedisDBConnect;
        private int _portNumber;
        private int _maxConnections;
        private bool _useUPnP;
        private bool _useWhiteList;
        private bool _configReadSuccess;
        private List<string> _whitelist;
        private List<string> _supportedHTML;
        private Dictionary<string, RedisAction> _redisActionKeyPair;
        private Dictionary<string, long> _actionThrottle;
        private string _redisEnvironmentKey;
        private string _version;
        private string _versionKey;

        public ConfigReader()
        {
            _configPath = Directory.GetCurrentDirectory() + "\\config.xml";
            XmlDocument xml = new XmlDocument();
            _redisActionKeyPair = new Dictionary<string, RedisAction>();
            _supportedHTML = new List<string>();
            _actionThrottle = new Dictionary<string, long>();
            try
            {
                xml.Load(_configPath);
                XmlNodeList dbxml = xml.GetElementsByTagName("DBConnect");
                XmlNodeList redisxml = xml.GetElementsByTagName("RedisDBConnect");
                XmlNodeList portxml = xml.GetElementsByTagName("Port");
                XmlNodeList maxConnxml = xml.GetElementsByTagName("MaxConnections");
                XmlNodeList whitelistxml = xml.GetElementsByTagName("WhiteList");
                XmlNodeList upnpxml = xml.GetElementsByTagName("UseUPnP");
                XmlNodeList actionkeysxml = xml.SelectNodes("/Config/RedisActionKeys/Pair");
                XmlNodeList supportedHTMLxml = xml.GetElementsByTagName("SupportedHTML");
                XmlNodeList redisEnvironmentxml = xml.GetElementsByTagName("RedisEnvironmentKey");
                XmlNodeList versionxml = xml.GetElementsByTagName("Version");
                XmlNodeList versionkeyxml = xml.GetElementsByTagName("VersionKey");
                XmlNodeList actionthrottlexml = xml.SelectNodes("/Config/Throttle/Action");

                if (dbxml.Count == 0)
                    throw new Exception("Could not find <DBConnect> in config");
                if (portxml.Count == 0)
                    throw new Exception("Could not find <Port> in config");
                if (maxConnxml.Count == 0)
                    throw new Exception("Could not find <MaxConnections> in config");
                if (redisxml.Count == 0)
                    throw new Exception("Could not find <RedisDBConnect> in config");
                if (redisEnvironmentxml.Count == 0)
                    throw new Exception("Could not find <RedisEnvironmentKey> in config");
                if (versionxml.Count == 0)
                    throw new Exception("Could not find <Version> in config");
                if (versionkeyxml.Count == 0)
                    throw new Exception("Could not find <VersionKey> in config");
                if (whitelistxml.Count == 0)
                    _useWhiteList = false;
                else
                    _useWhiteList = true;
                if (upnpxml.Count == 0)
                    _useUPnP = false;
         

                _MySQLDBConnect = dbxml[0].InnerText;
                _RedisDBConnect = redisxml[0].InnerText;
                _portNumber = int.Parse(portxml[0].InnerText);
                _maxConnections = int.Parse(maxConnxml[0].InnerText);

                if (_useWhiteList)
                    _whitelist = new List<String>(whitelistxml[0].InnerText.Split(';'));

                if (upnpxml.Count != 0 && (upnpxml[0].InnerText.ToUpper() == "YES" || upnpxml[0].InnerText.ToUpper() == "TRUE"))
                    _useUPnP = true;

                if (actionkeysxml.Count > 0)
                {
                    foreach (XmlNode x in actionkeysxml)
                    {
                        if (x.Attributes["Action"] != null && x.Attributes["RedisKey"] != null && x.Attributes["RedisAction"] != null)
                        {
                            _redisActionKeyPair.Add(x.Attributes["Action"].Value, 
                                new RedisAction(x.Attributes["RedisKey"].Value, x.Attributes["RedisAction"].Value));
                        }
                        else
                        {
                            throw new Exception("<RedisActionKeys><Pair> - xml malformed (missing attribute 'Action' or 'RedisKey' or 'RedisAction'");
                        }
                    }
                }

                if (actionthrottlexml.Count > 0)
                {
                    foreach (XmlNode x in actionthrottlexml)
                    {
                        if (x.Attributes["Name"] != null && x.Attributes["AveragePerSecondLimit"] != null)
                        {
                            _actionThrottle.Add(x.Attributes["Name"].Value, Convert.ToInt64(x.Attributes["AveragePerSecondLimit"].Value));
                        }
                        else
                        {
                            throw new Exception("<Throttle><Action> - xml malformed (missing attribute 'Name' or 'AveragePerSecondLimit'");
                        }
                    }
                }

                if (supportedHTMLxml.Count > 0)
                {
                    _supportedHTML = supportedHTMLxml[0].InnerText.Split(',').ToList<string>();
                }

                _redisEnvironmentKey = redisEnvironmentxml[0].InnerText;
                _version = versionxml[0].InnerText;
                _versionKey = versionkeyxml[0].InnerText;

                _configReadSuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - could not open or read config file (path: " + _configPath + ") - " + ex.Message);
                _configReadSuccess = false;
            }
        }

        int IConfigReader.PortNumber => _portNumber;
        int IConfigReader.MaxConnections => _maxConnections;
        string IConfigReader.MySQLDBConnect => _MySQLDBConnect;
        string IConfigReader.RedisDBConnect => _RedisDBConnect;
        bool IConfigReader.ConfigReadSuccess => _configReadSuccess;
        bool IConfigReader.UseUPnP => _useUPnP;
        bool IConfigReader.UseWhiteList => _useWhiteList;
        List<string> IConfigReader.WhiteList => _whitelist;
        List<string> IConfigReader.SupportedHTML => _supportedHTML;
        Dictionary<string, RedisAction> IConfigReader.RedisActionKeys => _redisActionKeyPair;
        string IConfigReader.RedisEnvironmentKey => _redisEnvironmentKey;
        string IConfigReader.Version => _version;
        string IConfigReader.VersionKey => _versionKey;
        Dictionary<string, long> IConfigReader.ActionThrottle => _actionThrottle;
    }
}
