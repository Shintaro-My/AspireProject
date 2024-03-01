using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace WebApi.Context
{
    public class SSEManagerContext
    {
        private readonly Dictionary<Guid, (Guid? userId, Queue<object> queue)> _queues = new();

        /// <summary>
        /// スレッドセーフな形で登録済みのQueueとUserIdを返却します
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<Guid, (Guid? userId, Queue<object> queue)> GetAll() => new(_queues);

        /// <summary>
        /// 一意のIDからQueueを取得します
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Queue<object> GetQueueById(Guid id) => _queues[id].queue;

        /// <summary>
        /// 新規にQueueとUserIdを登録します
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public Guid AddQueue(Guid? userId)
        {
            Guid id = Guid.NewGuid();
            _queues.TryAdd(id, (userId, new Queue<object>()));
            return id;
        }
        /// <summary>
        /// Queueの登録を解除します
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task RemoveQueue(Guid id)
        {
            if (_queues.ContainsKey(id))
            {
                _queues.Remove(id);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 合致するIDのQueueにメッセージを登録します
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void AddMsg(Guid id, object data)
        {
            var queue = GetQueueById(id);
            queue.Enqueue(data);
        }

        /// <summary>
        /// 登録されている全てのQueueにメッセージを登録します
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AddMsgAll(object data)
        {
            foreach (var key in _queues.Keys)
            {
                AddMsg(key, data);
            }
        }

        public IEnumerable<Guid> GetQueueIdsByUserId(Guid userId)
        {
            return _queues.Where(pair => pair.Value.userId == userId).Select(pair => pair.Key);
        }

        /// <summary>
        /// 一致するUserIdとともに登録されたQueueにメッセージを登録します
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AddMsgByUserIds(List<Guid> userIds, object data)
        {
            foreach (var userId in userIds)
            {
                foreach(var id in GetQueueIdsByUserId(userId).ToList())
                {
                    AddMsg(id, data);
                }
            }
        }

        /// <summary>
        /// Queueにメッセージが登録されている場合、それを送信します
        /// </summary>
        /// <param name="id"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public object? GetMsg(Guid id)
        {
            var queue = GetQueueById(id);
            if (queue.Count == 0) return null;
            return queue.Dequeue();
        }


    }
}
