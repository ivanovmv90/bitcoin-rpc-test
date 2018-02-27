using System;
using System.Net;
using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;
using NBXplorer.DerivationStrategy;
using NBXplorer.Models;

namespace BtcRpcTest
{
    class Program
    {
        static void Main(string[] args)
        {
                        
            var userName = "ceiwHEbqWI83";
            var userPass = "DwubwWsoo3";
            var rpcUri = "http://127.0.0.1:49372";
            var nbxplorerUri = "http://127.0.0.1:32838";
            
//            var key = ExtKey.Parse("tprv8ZgxMBicQKsPfFzQKRMAGhhpu4QwJvfYkEFTp8wxVjAAMuQMHQk3nTycDixnzLsLSbcSZqaf6QAZ7aq8EFNvk9boFeVTuwwatBnjUg8nYoQ");
            var key = ExtKey.Parse("tprv8ZgxMBicQKsPecJSUcMdXNCyVvjBi36cwC5ceRp6K8jYX9VvUTA1Agd473pLVYmUghc8RMEuBp7hih3CnWFknVGPcGvtCN8CiM1iv2yneSg");
            Console.WriteLine($"master key: {key.ToString(Network.RegTest)}, private key: {key.PrivateKey.ToString(Network.RegTest)}");
            
            var network = new NBXplorerNetworkProvider(ChainType.Regtest).GetBTC();
            var explorerClient = new ExplorerClient(network, new Uri(nbxplorerUri));
            var pubKey = explorerClient
                .Network
                .DerivationStrategyFactory
                .CreateDirectDerivationStrategy(key.Neuter(), new DerivationStrategyOptions
                {
                    Legacy = true
                });
            explorerClient.Track(pubKey);
            Console.WriteLine($"{pubKey.ToString()}, {explorerClient.CryptoCode}");

            var rpcClient = new RPCClient(new NetworkCredential(userName, userPass), new Uri(rpcUri), Network.RegTest);
            var balance = rpcClient.GetBalance();
            Console.WriteLine("rpc getbalance: " + balance.ToDecimal(MoneyUnit.BTC));
            
            //Try send
//            var address = btcKey.ExtKey.Derive(new KeyPath("0/2")).Neuter().PubKey.Hash.GetAddress(Network.RegTest);
//            var address = key.Derive(new KeyPath("0/2")).Neuter().PubKey.Hash.GetAddress(Network.RegTest);
            var utxos = explorerClient.GetUTXOs(pubKey, null, false);
//            Console.WriteLine($"Unspent coins: {coins.Length}");
            var address = explorerClient
                .GetUnused(pubKey, DerivationFeature.Direct)
                .ScriptPubKey
                .GetDestinationAddress(Network.RegTest);
//            Console.WriteLine(address.ToString());
            rpcClient.SendToAddress(address, Money.Coins(1m));
            utxos = explorerClient.GetUTXOs(pubKey, utxos);
//            explorerClient = new ExplorerClient(network, new Uri(nbxplorerUri));
            utxos = explorerClient.GetUTXOs(pubKey, null, true);
            var coins = utxos.GetUnspentCoins();
            Console.WriteLine($"Unspent coins: {coins.Length}");

//            var someAddress = neuteredKey.Derive(1).ScriptPubKey.GetDestinationAddress(Network.RegTest);
//            var tx = rpcClient.SendToAddress(someAddress, new Money(10, MoneyUnit.BTC));
//            Console.WriteLine($"sent coins via transaction {tx}");


//            var unusedAddr = explorerClient
//                .GetUnused(userDerivationScheme, DerivationFeature.Deposit)
//                .ScriptPubKey
//                .GetDestinationAddress(Network.RegTest);
//            Console.WriteLine(unusedAddr.ToString());
//            Console.ReadKey();
//            var tx = rpcClient.SendToAddress(unusedAddr, Money.Coins(10m));
//            Console.WriteLine($"sent coins via transaction {tx}");
            
//            var txId = uint256.Parse("0796d56e458a0186ce7d16e2c0e8b986e1c16dcb2907a46d544a8302287dacd4");
//            var transaction = explorerClient.GetTransaction(txId);
//            Console.WriteLine($"Transaction contains: {transaction.Transaction.TotalOut.ToDecimal(MoneyUnit.BTC)}");
//            utxos = explorerClient.GetUTXOs(userDerivationScheme, utxos, true);
//            utxos = explorerClient.GetUTXOs(userDerivationScheme, null, false);
//            Console.WriteLine($"Unspent coins: {utxos.GetUnspentCoins().Length}");
            Console.ReadKey();
        }
    }
}
