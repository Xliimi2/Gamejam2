using UnityEngine;
using Unity.Netcode;

public class MonsterAnimation : NetworkBehaviour
{
    // المتغير الشبكي للتحكم في حالة الأنمي (هل الوحش يهاجم أم لا)
    private NetworkVariable<bool> isAttacking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Animation monsterAnimation;

    private void Start()
    {
        // الحصول على مكون الأنمي للوحش
        monsterAnimation = GetComponent<Animation>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // فقط اللاعب الذي يملك الوحش سيقوم بتحديث الأنمي
            isAttacking.OnValueChanged += OnAttackStateChanged;
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            HandleMonsterAttack(); // تحديث حالة الهجوم بناءً على المدخلات
        }

        UpdateMonsterAnimation(); // تحديث الأنمي بناءً على الحالة
    }

    // وظيفة للتحقق إذا كان الوحش يهاجم
    private void HandleMonsterAttack()
    {
        if (Input.GetKeyDown(KeyCode.F)) // على سبيل المثال، الهجوم عند الضغط على المفتاح "F"
        {
            if (!isAttacking.Value)
            {
                isAttacking.Value = true;  // تغيير حالة الهجوم
            }
        }
        else
        {
            if (isAttacking.Value)
            {
                isAttacking.Value = false;  // إيقاف الهجوم
            }
        }
    }

    // تحديث الأنمي بناءً على حالة الهجوم
    private void UpdateMonsterAnimation()
    {
        if (monsterAnimation != null)
        {
            if (isAttacking.Value)
            {
                // إذا كان الوحش يهاجم، تأكد من تشغيل الأنميشن "Attack"
                if (!monsterAnimation.isPlaying || monsterAnimation["Attack"].time == 0)
                {
                    monsterAnimation.Play("Attack"); // تشغيل الأنميشن "Attack"
                }
            }
            else
            {
                // إذا توقفت الهجمات، إيقاف الأنميشن الحالي والرجوع إلى "Idle"
                monsterAnimation.Stop(); // إيقاف الأنميشن الحالي
                monsterAnimation.Play("Idle"); // تشغيل الأنميشن "Idle" عند التوقف
            }
        }
    }

    // تحديث الأنمي عند تغيير حالة الهجوم عبر الشبكة
    private void OnAttackStateChanged(bool oldState, bool newState)
    {
        if (IsOwner)  // تأكد من أن الأنمي يتم تحديثه فقط للاعب الذي يملكه
        {
            if (monsterAnimation != null)
            {
                if (newState)
                {
                    if (!monsterAnimation.isPlaying || monsterAnimation["Attack"].time == 0)
                    {
                        monsterAnimation.Play("Attack"); // تشغيل الأنمي "Attack"
                    }
                }
                else
                {
                    monsterAnimation.Stop("Attack"); // إيقاف الأنمي "Attack"
                }
            }
        }
    }
}
