namespace ServerCore;

public class ReadWriteLock
{
    const int EMPTY_FLAG = 0x00000000;

    private const int WRITE_MASK = 0x7FFF0000;

    private const int READ_MASK = 0x0000FFFF;

    private const int MAX_SPIN_COUNT = 5000;
    
    // [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
    private int _flag = EMPTY_FLAG;
    
    private int _writeCount = 0;

    public void WriteLock()
    {
        int threadId = (_flag & WRITE_MASK) >> 16;
        if (Thread.CurrentThread.ManagedThreadId  == threadId)
        {
            _writeCount++;
            return;
        }
        
        int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
        while (true)
        {
            for (int i = 0; i < MAX_SPIN_COUNT; i++)
            {
                if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                {
                    _writeCount = 1;
                    return;
                }
            }
            
            Thread.Yield();
        }
    }

    public void WriteUnlock()
    {
        int lockCount = --_writeCount;
        if (lockCount == 0)
        {
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }
    }

    public void ReadLock()
    {
        int threadId = (_flag & WRITE_MASK) >> 16;
        if (Thread.CurrentThread.ManagedThreadId  == threadId)
        {
            Interlocked.Increment(ref _flag);
            return;
        }
        while (true)
        {
            for (int i = 0; i < MAX_SPIN_COUNT; i++)
            {
                int expected = (_flag & READ_MASK);
                if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                {
                    _flag++;
                    return;
                }
            }
            
            Thread.Yield();
        }
    }

    public void ReadUnlock()
    {
        Interlocked.Decrement(ref _flag);
    }
}