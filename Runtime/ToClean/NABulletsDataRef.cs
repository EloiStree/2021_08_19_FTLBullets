
using System;
using Unity.Collections;

public class  NABulletsDataRef
{

    private uint m_elementMaxCount;
    public NativeArray<TriggeredBulletData> m_initBulletsParameterMemory;
    public NativeArray<BulletDataResult> m_computeResultMemory;

    public NABulletsDataRef(uint elementMaxCount)
    {
        InitWith(elementMaxCount);
    }
    public void InitWith(uint elementCount) {
        m_elementMaxCount = elementCount;
        TriggeredBulletData[] data = new TriggeredBulletData[elementCount];
        BulletDataResult[] dataResult = new BulletDataResult[elementCount];
        m_initBulletsParameterMemory = new NativeArray<TriggeredBulletData>(data, Allocator.Persistent);
        m_computeResultMemory = new NativeArray<BulletDataResult>(dataResult, Allocator.Persistent);
    }

    public void GetNativeArrayBulletData(out NativeArray<TriggeredBulletData> bulletsData)
    {
        bulletsData = m_initBulletsParameterMemory;
    }
    public void GetNativeArrayBulletData(out NativeArray<BulletDataResult> bulletsDataResult)
    {
        bulletsDataResult = m_computeResultMemory;
    }

    public void SetBulletAs(int index, bool isUsed)
    {
        bool change = false;
        TriggeredBulletData bullet = m_initBulletsParameterMemory[index];
        change = bullet.m_isActive != isUsed;
        bullet.m_isActive = isUsed;
        m_initBulletsParameterMemory[index] = bullet;

        if (change) { 
            BulletDataResult d = m_computeResultMemory[index];
            d.ResetAsAvailaible();
            m_computeResultMemory[index] = d;
        }
    }

    public void SetBullet(int index, TriggeredBulletData bullet)
    {
        m_initBulletsParameterMemory[index] = bullet;


    }
    public void GetBullet(int index, out TriggeredBulletData bullet)
    {
        bullet = m_initBulletsParameterMemory[index];
    }
    public void GetBulletResult(int index, out BulletDataResult bullet)
    {
        bullet = m_computeResultMemory[index];
    }


    public void Dispose()
    {
        m_computeResultMemory.Dispose();
        m_initBulletsParameterMemory.Dispose();

    }

    ~NABulletsDataRef()
    {
        Dispose();
    }
}