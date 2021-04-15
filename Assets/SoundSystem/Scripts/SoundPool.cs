using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class SoundPool
    {
        Transform _parent;

        private Queue<AudioSource> pool = new Queue<AudioSource>();

        public SoundPool(Transform parent, int startSize)
        {
            _parent = parent;

            CreateInitialPool(startSize);
        }
        //
        void CreateInitialPool(int startingPoolSize)
        {
            for (int i = 0; i < startingPoolSize; i++)
            {
                CreatePoolObject();
            }
        }

        public AudioSource Get()
        {
            if (pool.Count == 0)
            {
                CreatePoolObject();
            }

            AudioSource objectFromPool = pool.Dequeue();
            objectFromPool.gameObject.SetActive(true);

            return objectFromPool;
        }

        public void Return(AudioSource objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            pool.Enqueue(objectToReturn);
        }

        private void CreatePoolObject()
        {
            GameObject newGameObject = new GameObject("SoundSource");
            AudioSource newSource = newGameObject.AddComponent<AudioSource>();

            newGameObject.transform.SetParent(_parent);
            newGameObject.gameObject.SetActive(false);
            pool.Enqueue(newSource);
        }
    }
}


