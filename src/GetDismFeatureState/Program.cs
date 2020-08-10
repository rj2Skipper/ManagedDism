// Copyright (c). All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetDismFeatureState
{
    /// <summary>
    /// class GetDismFeatureState.Program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">array of command line arguments</param>
        /// <returns>default -1</returns>
        public static int Main(string[] args)
        {
            DismApi.Initialize(DismLogLevel.LogErrorsWarningsInfo);
            try
            {
                using (DismSession session = DismApi.OpenOnlineSession())
                {
                    if (args.Length > 0)
                    {
                        if (args[0] == "-info" && args.Length > 1)
                        {
                            string[] arg = args[1].Split(',');
                            List<DismFeatureInfo> dismFeatureInfos = new List<DismFeatureInfo>();
                            foreach (string a in arg)
                            {
                                DismFeatureInfo dismFeatureInfo = DismApi.GetFeatureInfo(session, a);
                                dismFeatureInfos.Add(dismFeatureInfo);
                            }

                            Console.WriteLine(dismFeatureInfos.ToArray().JsonSerialize());
                        }
                        else
                        {
                            string[] arg = args[0].Split(',');
                            if (arg.Length == 1)
                            {
                                DismFeatureInfo dismFeatureInfo = DismApi.GetFeatureInfo(session, arg[0]);
                                Console.WriteLine(dismFeatureInfo.JsonSerialize());
                                return (int)dismFeatureInfo.FeatureState;
                            }
                            else
                            {
                                DismFeatureCollection features = DismApi.GetFeatures(session);
                                var ss = from fe in features join ff in arg on fe.FeatureName equals ff select fe;
                                Console.WriteLine(ss.ToArray().JsonSerialize());
                            }
                        }
                    }
                    else
                    {
                        DismFeatureCollection features = DismApi.GetFeatures(session);
                        Console.WriteLine(features.ToArray().JsonSerialize());
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // Shut down the DismApi
                DismApi.Shutdown();
            }

            return -1;
        }
    }
}
