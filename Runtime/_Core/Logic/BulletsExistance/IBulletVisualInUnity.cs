using UnityEngine;

public interface IBulletVisualInUnity {

    void GetLinkedBulletTicket(out IBulletIdTicket ticket);
    void GetLinkedTransformRoot(out Transform root);
    void GetAllTransformAsArray(out Transform[] asArray);
}