using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AsyncTasks {

    // get UTC time from server
    public DateTime GetNISTDate(bool convertToLocalTime)
    {
        System.Random ran = new System.Random(DateTime.Now.Millisecond);
        DateTime date = DateTime.Today;
        string serverResponse = string.Empty;

        // list of NIST servers
        string[] servers = new string[] {
                         "129.6.15.28",
                         "129.6.15.29",
                         "129.6.15.30",
                         "98.175.203.200",
                         "66.199.22.67",
                         "64.113.32.5",
                         "24.56.178.140",
                         "128.138.140.44"
                          };

        // Try each server in random order to avoid blocked requests due to too frequent request
        for (int i = 0; i < 5; i++)
        {
            try
            {
                // Open a StreamReader to a random time server
                int randIdx = ran.Next(0, servers.Length);
                Debug.Log("POLLING SERVER: " + servers[randIdx]);
                StreamReader reader = new StreamReader(new System.Net.Sockets.TcpClient(servers[randIdx], 13).GetStream());
                serverResponse = reader.ReadToEnd();
                Debug.Log("SERVER RESPONSE: " + serverResponse);
                reader.Close();
                // Check to see that the signiture is there
                if (serverResponse.Length > 47 && serverResponse.Substring(38, 9).Equals("UTC(NIST)"))
                {
                    // Parse the date
                    int jd = int.Parse(serverResponse.Substring(1, 5));
                    int yr = int.Parse(serverResponse.Substring(7, 2));
                    int mo = int.Parse(serverResponse.Substring(10, 2));
                    int dy = int.Parse(serverResponse.Substring(13, 2));
                    int hr = int.Parse(serverResponse.Substring(16, 2));
                    int mm = int.Parse(serverResponse.Substring(19, 2));
                    int sc = int.Parse(serverResponse.Substring(22, 2));

                    if (jd > 51544)
                        yr += 2000;
                    else
                        yr += 1999;

                    date = new DateTime(yr, mo, dy, hr, mm, sc);

                    // Convert it to the current timezone if desired
                    if (convertToLocalTime)
                        date = date.ToLocalTime();

                    // Exit the loop
                    break;
                }

            }
            catch (Exception ex)
            {
                /* Do Nothing...try the next server */
                Debug.Log(ex);
            }
        }

        return date;
    }


}
