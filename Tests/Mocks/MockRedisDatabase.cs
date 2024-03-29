﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Tests.Mocks
{
    class MockRedisDatabase : IDatabase
    {
        public Dictionary<RedisKey, RedisValue> MockStringSetStore;
        public Dictionary<RedisKey, List<RedisValue>> MockListStore;
        public Dictionary<RedisKey, List<HashEntry>> MockHashStore;
        public Dictionary<RedisKey, TimeSpan?> MockDBStoreLife;
        public Dictionary<RedisChannel, RedisValue> MockChannel;
        public Dictionary<RedisChannel, int> MockChannelCount;
        int IDatabase.Database => 1;
        private IRedisExecuteBehaviour _redisBehaviour;
        private IRedisPublishBehaviour _redisPublishBehaviour;

        public MockRedisDatabase(IRedisExecuteBehaviour behaviour, IRedisPublishBehaviour publish)
        {
            _redisBehaviour = behaviour;
            _redisPublishBehaviour = publish;
            MockStringSetStore = new Dictionary<RedisKey, RedisValue>();
            MockDBStoreLife = new Dictionary<RedisKey, TimeSpan?>();
            MockListStore = new Dictionary<RedisKey, List<RedisValue>>();
            MockChannel = new Dictionary<RedisChannel, RedisValue>();
            MockChannelCount = new Dictionary<RedisChannel, int>();
            MockHashStore = new Dictionary<RedisKey, List<HashEntry>>();
        }

        bool IDatabase.StringSet(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags)
        {        
            MockStringSetStore[key] = value;
            MockDBStoreLife[key] = expiry;
            return _redisBehaviour.Execute(key, value, expiry, when, flags); 
        }

        bool IDatabase.StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when, CommandFlags flags)
        {    
            foreach (KeyValuePair<RedisKey,RedisValue> pair in values)
            {              
                MockStringSetStore[pair.Key] = pair.Value;
                MockDBStoreLife[pair.Key] = null;
            }
            return _redisBehaviour.Execute(values, when, flags);
        }

        long IDatabase.ListRightPush(RedisKey key, RedisValue value, When when, CommandFlags flags)
        {
            if (!MockListStore.ContainsKey(key))
                MockListStore[key] = new List<RedisValue>();

            MockListStore[key].Add(value);

            return MockListStore[key].Count;
        }

        long IDatabase.ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            if (!MockListStore.ContainsKey(key))
                MockListStore[key] = new List<RedisValue>();

            foreach (RedisValue v in values)
                MockListStore[key].Add(v);

            return MockListStore[key].Count;
        }

        void IDatabase.HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags)
        {
            if (!MockHashStore.ContainsKey(key))
                MockHashStore[key] = new List<HashEntry>();

            foreach (HashEntry v in hashFields)
                MockHashStore[key].Add(v);
        }

        bool IDatabase.HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when, CommandFlags flags)
        {
            if (!MockHashStore.ContainsKey(key))
                MockHashStore[key] = new List<HashEntry>();

            MockHashStore[key].Add(new HashEntry(hashField, value));

            return true;
        }

        long IDatabase.Publish(RedisChannel channel, RedisValue message, CommandFlags flags)
        {
            MockChannel[channel] = message;
            if (MockChannelCount.ContainsKey(channel))
                MockChannelCount[channel]++;
            else
                MockChannelCount[channel] = 1;
            return _redisPublishBehaviour.Execute(channel, message, flags);
        }

        Task<long> IDatabaseAsync.PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags)
        {
            MockChannel[channel] = message;
            if (MockChannelCount.ContainsKey(channel))
                MockChannelCount[channel]++;
            else
                MockChannelCount[channel] = 1;
            return new Task<long>(() => _redisPublishBehaviour.Execute(channel, message, flags));
        }

        bool IDatabase.HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags)
        {
            if (MockHashStore.ContainsKey(key))
                MockHashStore[key].RemoveAll(k => k.Name == hashField);
            return true;
        }

        long IDatabase.HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags)
        {
            if (MockHashStore.ContainsKey(key))
            {
                foreach (RedisValue v in hashFields)
                {
                    MockHashStore[key].RemoveAll(k => k.Name == v);
                }
            }
            return 1;  
        }

        #region UnimplementedMockMethods

        ConnectionMultiplexer IRedisAsync.Multiplexer => throw new NotImplementedException();

        IBatch IDatabase.CreateBatch(object asyncState)
        {
            throw new NotImplementedException();
        }

        ITransaction IDatabase.CreateTransaction(object asyncState)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.DebugObject(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.DebugObjectAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisResult IDatabase.Execute(string command, params object[] args)
        {
            throw new NotImplementedException();
        }

        RedisResult IDatabase.Execute(string command, ICollection<object> args, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisResult> IDatabaseAsync.ExecuteAsync(string command, params object[] args)
        {
            throw new NotImplementedException();
        }

        Task<RedisResult> IDatabaseAsync.ExecuteAsync(string command, ICollection<object> args, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.GeoAdd(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.GeoAdd(RedisKey key, GeoEntry value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.GeoAdd(RedisKey key, GeoEntry[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.GeoAddAsync(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.GeoAddAsync(RedisKey key, GeoEntry value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.GeoAddAsync(RedisKey key, GeoEntry[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double? IDatabase.GeoDistance(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double?> IDatabaseAsync.GeoDistanceAsync(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        string[] IDatabase.GeoHash(RedisKey key, RedisValue[] members, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        string IDatabase.GeoHash(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<string[]> IDatabaseAsync.GeoHashAsync(RedisKey key, RedisValue[] members, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<string> IDatabaseAsync.GeoHashAsync(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        GeoPosition?[] IDatabase.GeoPosition(RedisKey key, RedisValue[] members, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        GeoPosition? IDatabase.GeoPosition(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<GeoPosition?[]> IDatabaseAsync.GeoPositionAsync(RedisKey key, RedisValue[] members, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<GeoPosition?> IDatabaseAsync.GeoPositionAsync(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        GeoRadiusResult[] IDatabase.GeoRadius(RedisKey key, RedisValue member, double radius, GeoUnit unit, int count, Order? order, GeoRadiusOptions options, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        GeoRadiusResult[] IDatabase.GeoRadius(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit, int count, Order? order, GeoRadiusOptions options, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<GeoRadiusResult[]> IDatabaseAsync.GeoRadiusAsync(RedisKey key, RedisValue member, double radius, GeoUnit unit, int count, Order? order, GeoRadiusOptions options, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<GeoRadiusResult[]> IDatabaseAsync.GeoRadiusAsync(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit, int count, Order? order, GeoRadiusOptions options, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.GeoRemove(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.GeoRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.HashDecrement(RedisKey key, RedisValue hashField, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double IDatabase.HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.HashDecrementAsync(RedisKey key, RedisValue hashField, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double> IDatabaseAsync.HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.HashExists(RedisKey key, RedisValue hashField, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.HashGet(RedisKey key, RedisValue hashField, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        HashEntry[] IDatabase.HashGetAll(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<HashEntry[]> IDatabaseAsync.HashGetAllAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.HashIncrement(RedisKey key, RedisValue hashField, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double IDatabase.HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.HashIncrementAsync(RedisKey key, RedisValue hashField, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double> IDatabaseAsync.HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.HashKeys(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.HashKeysAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.HashLength(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.HashLengthAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        IEnumerable<HashEntry> IDatabase.HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        IEnumerable<HashEntry> IDatabase.HashScan(RedisKey key, RedisValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseAsync.HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.HashValues(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.HashValuesAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.HyperLogLogLength(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.HyperLogLogLength(RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.HyperLogLogLengthAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        void IDatabase.HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        void IDatabase.HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseAsync.HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseAsync.HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        EndPoint IDatabase.IdentifyEndpoint(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<EndPoint> IDatabaseAsync.IdentifyEndpointAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabaseAsync.IsConnected(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.KeyDelete(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.KeyDelete(RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.KeyDeleteAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.KeyDeleteAsync(RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        byte[] IDatabase.KeyDump(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<byte[]> IDatabaseAsync.KeyDumpAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.KeyExists(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.KeyExistsAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        void IDatabase.KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase, int timeoutMilliseconds, MigrateOptions migrateOptions, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseAsync.KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase, int timeoutMilliseconds, MigrateOptions migrateOptions, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.KeyMove(RedisKey key, int database, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.KeyMoveAsync(RedisKey key, int database, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.KeyPersist(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.KeyPersistAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisKey IDatabase.KeyRandom(CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisKey> IDatabaseAsync.KeyRandomAsync(CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.KeyRename(RedisKey key, RedisKey newKey, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.KeyRenameAsync(RedisKey key, RedisKey newKey, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        void IDatabase.KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseAsync.KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        TimeSpan? IDatabase.KeyTimeToLive(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<TimeSpan?> IDatabaseAsync.KeyTimeToLiveAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisType IDatabase.KeyType(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisType> IDatabaseAsync.KeyTypeAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.ListGetByIndex(RedisKey key, long index, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.ListLeftPop(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.ListLeftPopAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.ListLeftPush(RedisKey key, RedisValue value, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListLeftPushAsync(RedisKey key, RedisValue value, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.ListLength(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListLengthAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.ListRange(RedisKey key, long start, long stop, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.ListRangeAsync(RedisKey key, long start, long stop, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.ListRemove(RedisKey key, RedisValue value, long count, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListRemoveAsync(RedisKey key, RedisValue value, long count, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.ListRightPop(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.ListRightPopAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListRightPushAsync(RedisKey key, RedisValue value, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        void IDatabase.ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseAsync.ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        void IDatabase.ListTrim(RedisKey key, long start, long stop, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseAsync.ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.LockQuery(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.LockQueryAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.LockRelease(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        TimeSpan IRedis.Ping(CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<TimeSpan> IRedisAsync.PingAsync(CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisResult IDatabase.ScriptEvaluate(string script, RedisKey[] keys, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisResult IDatabase.ScriptEvaluate(byte[] hash, RedisKey[] keys, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisResult IDatabase.ScriptEvaluate(LuaScript script, object parameters, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisResult IDatabase.ScriptEvaluate(LoadedLuaScript script, object parameters, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisResult> IDatabaseAsync.ScriptEvaluateAsync(string script, RedisKey[] keys, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisResult> IDatabaseAsync.ScriptEvaluateAsync(byte[] hash, RedisKey[] keys, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisResult> IDatabaseAsync.ScriptEvaluateAsync(LuaScript script, object parameters, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisResult> IDatabaseAsync.ScriptEvaluateAsync(LoadedLuaScript script, object parameters, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.SetAdd(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.SetContains(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SetLength(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SetLengthAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.SetMembers(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SetMembersAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.SetPop(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.SetPopAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.SetRandomMember(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.SetRandomMemberAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.SetRandomMembers(RedisKey key, long count, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.SetRemove(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        IEnumerable<RedisValue> IDatabase.SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        IEnumerable<RedisValue> IDatabase.SetScan(RedisKey key, RedisValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.Sort(RedisKey key, long skip, long take, Order order, SortType sortType, RedisValue by, RedisValue[] get, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortAndStore(RedisKey destination, RedisKey key, long skip, long take, Order order, SortType sortType, RedisValue by, RedisValue[] get, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortAndStoreAsync(RedisKey destination, RedisKey key, long skip, long take, Order order, SortType sortType, RedisValue by, RedisValue[] get, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SortAsync(RedisKey key, long skip, long take, Order order, SortType sortType, RedisValue by, RedisValue[] get, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.SortedSetAdd(RedisKey key, RedisValue member, double score, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetAdd(RedisKey key, SortedSetEntry[] values, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights, Aggregate aggregate, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights, Aggregate aggregate, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double IDatabase.SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double> IDatabaseAsync.SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double IDatabase.SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double> IDatabaseAsync.SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetLength(RedisKey key, double min, double max, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetLengthAsync(RedisKey key, double min, double max, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.SortedSetRangeByRank(RedisKey key, long start, long stop, Order order, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SortedSetRangeByRankAsync(RedisKey key, long start, long stop, Order order, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        SortedSetEntry[] IDatabase.SortedSetRangeByRankWithScores(RedisKey key, long start, long stop, Order order, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<SortedSetEntry[]> IDatabaseAsync.SortedSetRangeByRankWithScoresAsync(RedisKey key, long start, long stop, Order order, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.SortedSetRangeByScore(RedisKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SortedSetRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        SortedSetEntry[] IDatabase.SortedSetRangeByScoreWithScores(RedisKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<SortedSetEntry[]> IDatabaseAsync.SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.SortedSetRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.SortedSetRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long? IDatabase.SortedSetRank(RedisKey key, RedisValue member, Order order, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long?> IDatabaseAsync.SortedSetRankAsync(RedisKey key, RedisValue member, Order order, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        IEnumerable<SortedSetEntry> IDatabase.SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        IEnumerable<SortedSetEntry> IDatabase.SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double? IDatabase.SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double?> IDatabaseAsync.SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringAppend(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringBitCount(RedisKey key, long start, long end, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringBitCountAsync(RedisKey key, long start, long end, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringBitPosition(RedisKey key, bool bit, long start, long end, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringBitPositionAsync(RedisKey key, bool bit, long start, long end, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringDecrement(RedisKey key, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double IDatabase.StringDecrement(RedisKey key, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringDecrementAsync(RedisKey key, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double> IDatabaseAsync.StringDecrementAsync(RedisKey key, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.StringGet(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue[] IDatabase.StringGet(RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.StringGetAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue[]> IDatabaseAsync.StringGetAsync(RedisKey[] keys, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.StringGetBit(RedisKey key, long offset, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.StringGetBitAsync(RedisKey key, long offset, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.StringGetRange(RedisKey key, long start, long end, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.StringGetSet(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValueWithExpiry IDatabase.StringGetWithExpiry(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValueWithExpiry> IDatabaseAsync.StringGetWithExpiryAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringIncrement(RedisKey key, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        double IDatabase.StringIncrement(RedisKey key, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringIncrementAsync(RedisKey key, long value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<double> IDatabaseAsync.StringIncrementAsync(RedisKey key, double value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        long IDatabase.StringLength(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<long> IDatabaseAsync.StringLengthAsync(RedisKey key, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IDatabase.StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDatabaseAsync.StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        RedisValue IDatabase.StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        Task<RedisValue> IDatabaseAsync.StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags)
        {
            throw new NotImplementedException();
        }

        bool IRedisAsync.TryWait(Task task)
        {
            throw new NotImplementedException();
        }

        void IRedisAsync.Wait(Task task)
        {
            throw new NotImplementedException();
        }

        T IRedisAsync.Wait<T>(Task<T> task)
        {
            throw new NotImplementedException();
        }

        void IRedisAsync.WaitAll(params Task[] tasks)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
