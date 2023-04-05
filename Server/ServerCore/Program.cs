using System;

namespace ServerCore;

internal class SpinLock
{
    private volatile int _lock;

    public void Acquire()
    {
        var expected = 0;
        var desired = 1;
        while (true)
            if (Interlocked.CompareExchange(ref _lock, desired, expected) == expected)
                break;
    }

    public void UnAcquire()
    {
        _lock = 0;
    }
}

internal class Program
{
    //private static SpinLock _spinLock = new();
    private static volatile int _num = 0;
    private static ReadWriteLock _readWriteLock = new ReadWriteLock();
    
    // private static void Thread1()
    // {
    //     for (var i = 0; i < 10000000; i++)
    //     {
    //         _spinLock.Acquire();
    //         _num += 1;
    //         _spinLock.UnAcquire();
    //     }
    // }

    // private static void Thread2()
    // {
    //     for (var i = 0; i < 10000000; i++)
    //     {
    //         _spinLock.Acquire();
    //         _num -= 1;
    //         _spinLock.UnAcquire();
    //     }
    // }

    private static void Main(string[] args)
    {
        var t1 = new Task(delegate
        {
            for (int i = 0; i < 1000000; i++)
            {
                _readWriteLock.WriteLock();
                _num++;
                _readWriteLock.WriteUnlock();
            }
        });
        var t2 = new Task(delegate
        {
            for (int i = 0; i < 1000000; i++)
            {
                _readWriteLock.WriteLock();
                _num--;
                _readWriteLock.WriteUnlock();
            }
        });
        t1.Start();
        t2.Start();
        Task.WaitAll(t1, t2);

        Console.WriteLine("Num = {0}", _num);
    }
}