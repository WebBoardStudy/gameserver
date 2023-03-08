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
    private static SpinLock _spinLock = new();
    private static int _num = 0;

    private static void Thread1()
    {
        for (var i = 0; i < 10000000; i++)
        {
            _spinLock.Acquire();
            _num += 1;
            _spinLock.UnAcquire();
        }
    }

    private static void Thread2()
    {
        for (var i = 0; i < 10000000; i++)
        {
            _spinLock.Acquire();
            _num -= 1;
            _spinLock.UnAcquire();
        }
    }

    private static void Main(string[] args)
    {
        var t1 = new Task(Thread1);
        var t2 = new Task(Thread2);
        t1.Start();
        t2.Start();
        Task.WaitAll(t1, t2);

        Console.WriteLine("Num = {0}", _num);
    }
}