using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Common
{
    public class Benchmark : MonoBehaviour
    {
        private static Benchmark mInstance;
        public static Benchmark Instance
        {
            get
            {
                if (mInstance == null)
                {
                    var obj = new GameObject("Benchmark");
                    mInstance = obj.AddComponent<Benchmark>();
                    mInstance.enabled = false;
                }
                return mInstance;
            }
        }

        public int fps;
        public int minFps;
        public int maxFps;

        private Action<int, int, int> mOnFinishedBenchmark;
        private float mElapsedTime;
        private int mCountFrame;
        private float mDelayStart;
        private float mBenchmarkDuration;
        private float mDelayWait;

        private void Start()
        {
            if (mInstance == null)
                mInstance = this;
            else if (mInstance != this)
                Destroy(gameObject);
        }

        private void Update()
        {
            if (mDelayStart > 0)
            {
                mDelayWait += Time.deltaTime;
                if (mDelayWait < mDelayStart)
                    return;
            }
            
            mElapsedTime += Time.deltaTime;
            mCountFrame++;
            if (mElapsedTime >= 1)
            {
                fps = Mathf.RoundToInt(mCountFrame * 1f / mElapsedTime);
                if (fps > maxFps) maxFps = fps;
                if (fps < minFps) minFps = fps;

                mElapsedTime = 0;
                mCountFrame = 0;
            }
            mBenchmarkDuration -= Time.deltaTime;
            if (mBenchmarkDuration <= 0)
            {
                mOnFinishedBenchmark(fps, minFps, maxFps);
                enabled = false;
            }
        }

        public void StartBenchmark(float pDuration, Action<int, int, int> pOnFinishedBenchmark, float pDelayStart = 0)
        {
            minFps = 1000;
            maxFps = 0;
            mDelayStart = pDelayStart;
            mBenchmarkDuration = pDuration;
            mOnFinishedBenchmark = pOnFinishedBenchmark;
            enabled = true;
        }
    }
}