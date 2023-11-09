using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestTimer : IDisposable
{
    public string TestName { get; private set; }
    public int TestCount {get; private set;}
    private readonly Stopwatch stopwatch;
    private bool isDisposed = false;


    public TestTimer(string tsetName, int testCount = 1)
    {
        if (testCount <= 0) throw new ArgumentException("TestCount must bigger than 0");

        TestName = tsetName;
        TestCount = testCount;
        stopwatch = Stopwatch.StartNew();
    }


    public void Dispose()
    {
        if (isDisposed) return;
        isDisposed = true;

        stopwatch.Stop();
        double time = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"{TestName} 运行了 {TestCount}次, 平均时间: {time / TestCount: 0.0000}ms, 总时间: {time: 0.00}ms");
    }
}
