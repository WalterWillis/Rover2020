﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RoverMobile.Models;
using RoverMobileGrpcClient;
using Xamarin.Forms;

namespace RoverMobile.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        private List<Item> items;
        private Item selectedItem;

        public MockDataStore()
        {
            if (items == null)
            {
                items = new List<Item>();
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task SelectItem(string id)
        {
            selectedItem = await GetItemAsync(id);
        }

        public async Task<Item> GetSelectedItem()
        {
            return await Task.FromResult(selectedItem);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async IAsyncEnumerable<string> GetIPAddresses([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var _interface = NetworkInterface
                           .GetAllNetworkInterfaces()
                           .Where(n => n.Description.Contains("wlan"))
                           .FirstOrDefault();

            var ip = _interface.GetIPProperties().UnicastAddresses
                .Where(n => n.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).SingleOrDefault();


            Byte[] maskBytes = ip.IPv4Mask.GetAddressBytes();
            Byte[] addrBytes = ip.Address.GetAddressBytes();

            byte[] networkAddrBytes = new Byte[maskBytes.Length]; // the network address such as 192.168.1.0
            byte[] broadcast = new byte[maskBytes.Length];

            int hostPossibilities = 0;
            for (int i = 0; i < maskBytes.Length; i++)
            {
                networkAddrBytes[i] = (byte)(maskBytes[i] & addrBytes[i]); // AND to zero out
                byte hostByte = (byte)(255 - maskBytes[i]); // get the host byte

                // if the host byte is 0, use the current address's byte so the 
                // result is the host broadcast address for the current subnet
                if(hostByte == 0)
                {
                    broadcast[i] = addrBytes[i];
                }
                else
                {
                    // multiply subnets together so we can grab the total number of hosts to scan.
                    hostPossibilities = hostPossibilities == 0 ? (hostByte) : (hostByte * hostPossibilities);

                    //get the broadcast of the current subnet
                    byte broadcastByte = (byte)((Byte)hostByte | addrBytes[i]);
                    broadcast[i] = broadcastByte;
                }
            }

            hostPossibilities -= 2; // remove the default gateway and broadcast address from the possibilities. Network aleady not considered.


            IPAddress networkAddr = new IPAddress(networkAddrBytes);
            IPAddress networkBroadcastAddr = new IPAddress(broadcast);

            List<string> currentItems = new List<string>(); //

            foreach (Item item in items)
                currentItems.Add(item.Text); //add the current list of IPs to our checklist so we don't waste time or get redundant IPs.

            IPAddress currentAttempt = GetNextIpAddress(networkAddr);
            currentAttempt = GetNextIpAddress(currentAttempt); // skip default gateway, assuming typical network

#if DEBUG
            //try the emulator address
            string debugHost = "10.0.2.2:5443";
            if (await Client.TryConnection(debugHost))
            {
                if (!currentItems.Contains(debugHost))
                {
                    yield return $"Currently Scanning {debugHost}";
                    Item newItem = new Item()
                    {
                        Description = "DEBUG address",
                        Id = Guid.NewGuid().ToString(),
                        Text = debugHost
                    };
                   
                    items.Add(newItem);
                    yield return $"found:{newItem.Id}";
                }
            }

            hostPossibilities = 15;
#endif

            for (int i = 0; i < hostPossibilities; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                string host = currentAttempt.ToString() + ":5443";

                if (!currentItems.Contains(host))
                {
                   yield return $"Currently Scanning {host}";
                    bool success = await Client.TryConnection(host);

                    if (success)
                    {
                        Item newItem = new Item()
                        {
                            Description = currentAttempt.ToString(),
                            Id = Guid.NewGuid().ToString(),
                            Text = host
                        };     
                        
                        items.Add(newItem);
                        yield return $"found:{newItem.Id}";
                    }
                }
                currentAttempt = GetNextIpAddress(currentAttempt);
            }

            yield return $"Finished Scanning";
        }

        private IPAddress GetNextIpAddress(IPAddress ipAddress)
        {
            byte[] addressBytes = ipAddress.GetAddressBytes().Reverse().ToArray();
            uint ipAsUint = BitConverter.ToUInt32(addressBytes, 0);
            byte[] nextAddress = BitConverter.GetBytes(ipAsUint + 1).Reverse().ToArray();
            return new IPAddress(nextAddress);
        }

    }
}