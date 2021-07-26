﻿/*
 * Copyright 2020-2021 VMware, Inc.
 * SPDX-License-Identifier: EPL-2.0
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SingletonClient;
using SingletonClient.Implementation.Support;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace UnitTestSingleton
{
    class BaseIo : ISingletonBaseIo, IAccessService
    {
        private static BaseIo _instance = new BaseIo();
        public static BaseIo obj()
        {
            return _instance;
        }

        private ResourceManager resourceManager;
        private Hashtable responseData = new Hashtable();
        private Hashtable testData = new Hashtable();

        private string lastConsoleText;

        public BaseIo()
        {
            Assembly assembly = typeof(BaseIo).Assembly;
            resourceManager = new ResourceManager("UnitTestSingleton.testdata.TestData", assembly);

            CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            ResourceSet resourceSet = resourceManager.GetResourceSet(cultureInfo, true, true);

            I18N.GetExtension().RegisterAccessService(this, "test");

            string raw = (string)resourceManager.GetObject("http_response");

            for (int k=1; k<10; k++)
            {
                string product = "CSHARP" + k;
                string text = raw.Replace("$PRODUCT", product).Replace("$VERSION", "1.0.0");
                string[] parts = Regex.Split(text, "---api---.*[\r|\n]*");
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Trim().Length > 5)
                    {
                        this.LoadResponse(parts[i]);
                    }
                }
            }

            PrepareTestData("test_define");
            PrepareTestData("test_define2");
        }

        private void LoadResponse(string apiData)
        {
            string[] parts = Regex.Split(apiData, "---data---.*[\r|\n]*");
            string[] segs = Regex.Split(parts[0], "---header---.*[\r|\n]*");
            string[] lines = Regex.Split(segs[0], "\n");

            string key = lines[0].Trim();
            if(segs.Length > 1 && segs[1].Trim().Length > 2)
            {
                JObject header = JObject.Parse(segs[1]);
                if (header != null)
                {
                    string tail = JsonConvert.SerializeObject(header);
                    key += "<<headers>>" + tail;
                }
            }

            Hashtable response = new Hashtable();
            segs = Regex.Split(parts[1], "---header---.*[\r|\n]*");
            response["body"] = segs[0].Trim();
            if (segs.Length > 1 && segs[1].Trim().Length > 2)
            {
                string[] pieces = Regex.Split(segs[1], "---code---.*[\r|\n]*");
                JObject header = JObject.Parse(pieces[0]);
                response["headers"] = header;
                if (pieces.Length > 1)
                {
                    response["code"] = Convert.ToInt32(pieces[1]);
                }
            }
            responseData.Add(key, response);
        }

        private void PrepareTestData(string resName)
        {
            string raw = (string)resourceManager.GetObject(resName);
            string[] parts = Regex.Split(raw, "---test---.*[\r|\n]*");
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Trim().Length == 0)
                {
                    continue;
                }
                string[] segs = Regex.Split(parts[i], "---data---.*[\r|\n]*");
                SingletonParserProperties p = new SingletonParserProperties();
                Hashtable ht = p.Parse(segs[0]);
                string name = (string)ht["NAME"];
                testData.Add(name, ht);

                for(int k=1; k<segs.Length; k++)
                {
                    Hashtable htTest = p.Parse(segs[k]);
                    ht.Add("_test_" + k, htTest);
                }
                ht["_test_count"] = segs.Length - 1;

                string dataFrom = (string)ht["DATAFROM"];
                if (!string.IsNullOrEmpty(dataFrom))
                {
                    Hashtable htFrom = (Hashtable)testData[dataFrom];
                    for (int k = 1; ; k++)
                    {
                        Hashtable oneData = (Hashtable)htFrom["_test_" + k];
                        if (oneData == null)
                        {
                            ht["_test_count"] = k - 1;
                            break;
                        }
                        ht["_test_" + k] = oneData;
                    }
                }
            }
        }

        public Hashtable GetTestData(string name)
        {
            return (Hashtable)testData[name];
        }

        public string GetTestResource(string name)
        {
            string str = null;
            object obj = resourceManager.GetObject(name);
            string type = obj.GetType().ToString();
            if (type.Equals("System.Byte[]"))
            {
                str = System.Text.Encoding.UTF8.GetString((System.Byte[])obj);
            }
            else
            {
                str = (string)obj;
            }

            return str.Trim();
        }

        public void ConsoleWriteLine(string text)
        {
            lastConsoleText = text;
            Console.WriteLine(text);
        }

        /// <summary>
        /// ISingletonBaseIo
        /// </summary>
        public string GetLastConsoleText()
        {
            return lastConsoleText;
        }

        private string GetResponse(string key, Hashtable headers)
        {
            Hashtable response = (Hashtable)responseData[key];
            string text = "";
            if (response != null)
            {
                text = (string)response["body"];
            }
            return text;
        }

        /// <summary>
        /// IAccessService
        /// </summary>
        public string HttpGet(string url, Hashtable headers)
        {
            return this.GetResponse("[GET]" + url, headers);
        }

        /// <summary>
        /// IAccessService
        /// </summary>
        public string HttpPost(string url, string text, Hashtable headers)
        {
            return this.GetResponse("[POST]" + url, headers);
        }
    }
}