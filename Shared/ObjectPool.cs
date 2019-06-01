using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotGameShared {
    public abstract class Poolable<TSelf> where TSelf : Poolable<TSelf> {
        private ObjectPool<TSelf> pool;

        public Poolable(ObjectPool<TSelf> pool) {
            this.pool = pool;
        }

        public void Free() => pool.Free((TSelf)this);
        public abstract void Reset();
    }

    public class ObjectPool<T> where T : Poolable<T> {
        private Stack<T> available = new Stack<T>();

        private readonly Func<ObjectPool<T>, T> create;

        public ObjectPool(Func<ObjectPool<T>, T> create) {
            this.create = create;
        }

        public T Create() {
            if (available.Any()) {
                return available.Pop();
            } else {
                return create(this);
            }
        }

        public void Free(T obj) {
            obj.Reset();
            available.Push(obj);
        }
    }
}
