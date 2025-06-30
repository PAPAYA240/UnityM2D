using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Manager : MonoBehaviour
{
    public class ParticlePoolInfo
    {
        public ParticleSystem particlePrefab; 
        public int initialPoolSize = 5;     
    }

    public List<ParticlePoolInfo> particlePools; // 여러 종류의 파티클 풀 설정

    private Dictionary<string, Queue<ParticleSystem>> _pools = new Dictionary<string, Queue<ParticleSystem>>();
    private Dictionary<string, ParticleSystem> _particlePrefabs = new Dictionary<string, ParticleSystem>();

    void InitializePools()
    {
        foreach (var poolInfo in particlePools)
        {
            string key = poolInfo.particlePrefab.name;
            _pools[key] = new Queue<ParticleSystem>();
            _particlePrefabs[key] = poolInfo.particlePrefab;

            for (int i = 0; i < poolInfo.initialPoolSize; i++)
            {
                ParticleSystem newParticle = Instantiate(poolInfo.particlePrefab, transform);
                newParticle.gameObject.SetActive(false); // 비활성화 상태로 풀에 추가
                newParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // 혹시 모를 재생 중인 파티클 정지 및 클리어
                _pools[key].Enqueue(newParticle);
            }
        }
    }

    public ParticleSystem PlayParticle(string particleName, Vector3 position, Quaternion rotation)
    {
        if (!_pools.ContainsKey(particleName))
        {
            Debug.LogWarning($"Particle system '{particleName}' not found in pool. Make sure it's added to ParticlePools list.");
            return null;
        }

        Queue<ParticleSystem> pool = _pools[particleName];
        ParticleSystem particleToPlay;

        if (pool.Count > 0)
        {
            particleToPlay = pool.Dequeue();
        }
        else // 풀에 여유가 없으면 새로 생성 (풀 확장)
        {
            Debug.LogWarning($"Pool for '{particleName}' exhausted. Expanding pool.");
            particleToPlay = Instantiate(_particlePrefabs[particleName], transform);
        }

        particleToPlay.transform.position = position;
        particleToPlay.transform.rotation = rotation;
        particleToPlay.gameObject.SetActive(true);
        particleToPlay.Play();

        var mainModule = particleToPlay.main;
        if (!mainModule.loop)
        {
            StartCoroutine(ReturnParticleToPoolAfterCompletion(particleToPlay, particleName, mainModule.duration));
        }
        else
        {
            Debug.LogWarning($"Playing looping particle '{particleName}'. You need to manually call StopAndReturnParticle() when done.");
        }

        return particleToPlay;
    }

    private IEnumerator ReturnParticleToPoolAfterCompletion(ParticleSystem particle, string particleName, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (particle.isPlaying || particle.gameObject.activeSelf)
        {
            StopAndReturnParticle(particle, particleName);
        }
    }

    // 외부에서 파티클을 멈추고 풀에 반환할 때 사용
    public void StopAndReturnParticle(ParticleSystem particle, string particleName)
    {
        if (particle == null || !_pools.ContainsKey(particleName)) return;

        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); 
        particle.gameObject.SetActive(false); // 비활성화
        _pools[particleName].Enqueue(particle); // 풀에 반환
    }
}
