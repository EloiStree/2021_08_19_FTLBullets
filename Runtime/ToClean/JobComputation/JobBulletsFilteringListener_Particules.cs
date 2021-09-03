using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

//DEVELOPER NOTE: Good idea for 2D games but don't work for large distance games.
public class JobBulletsFilteringListener_Particules : AbstractBulletsToRenderListener
{
    public ParticleSystem m_particuleSystem;

    public int m_maxParticulesWanted = 10000;
    NativeArray<ParticleSystem.Particle> m_particles;
    public float m_Drift = 0.01f;


    public override void InitWithCount(int count)
    {
        int particuleCount = m_maxParticulesWanted;
        if (count < m_maxParticulesWanted)
            particuleCount = count;
        ParticleSystem.Particle[] particules = new ParticleSystem.Particle[particuleCount];
        m_particles = new NativeArray<ParticleSystem.Particle>(particules, Allocator.Persistent);
        
    }

    public override void ApplyComputeRendering(ref FilteredBulletsId bulletsToProcess)
    {
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = m_particuleSystem.GetParticles(m_particles);

        ParticleSystem.Particle p;
        int pCount = m_particles.Length;
        int c = 0;
        for (int i = 0; i < m_bulletsRenderingRef.Length; i++)
        {
            if (c >= pCount)
                break;

            if (m_bulletsRenderingRef[i].m_canBeRender &&
                m_bulletsRenderingRef[i].m_distanceType == BulletDistanceType.Far)
            {
                p = m_particles[c];
                p.position = m_bulletsRef[i].m_currentPosition;
                m_particles[c] = p;
                c++;
            }
        }

        m_particuleSystem.SetParticles(m_particles, numParticlesAlive);
    }
}


public interface IBulletsToRenderListener {
    void InitWithCount(int count);
    void SetBulletsInformatoinRef(NativeArray<TriggeredBulletData> bulletInit, NativeArray<BulletDataResult> bulletsResult, NativeArray<BulletRendering> bulletsRendering);
    void ApplyComputeRendering(ref FilteredBulletsId bulletsToProcess);
}

public abstract class AbstractBulletsToRenderListener : MonoBehaviour, IBulletsToRenderListener
{
    protected NativeArray<BulletDataResult> m_bulletsRef;
    protected NativeArray<BulletRendering> m_bulletsRenderingRef;
    protected NativeArray<TriggeredBulletData> m_bulletInitRef;
    public abstract void ApplyComputeRendering(ref FilteredBulletsId bulletsToProcess );
    public abstract void InitWithCount(int count);
    public  void SetBulletsInformatoinRef(NativeArray<TriggeredBulletData> bulletInit,NativeArray<BulletDataResult> bulletsResult, NativeArray<BulletRendering> bullets)
    {
        m_bulletsRef = bulletsResult;
        m_bulletsRenderingRef = bullets;
        m_bulletInitRef = bulletInit;
        InitWithCount(m_bulletsRef.Length);
    }
}