using System;
using UnityEngine;

namespace Features.Common {
[Serializable]
public struct SerialisableVector3 {
    public float x;
    public float z;
    public float y;

    public SerialisableVector3(float z, float y, float x) {
        this.z = z;
        this.y = y;
        this.x = x;
    }

    public Vector3 To() {
        return new Vector3(x, y, z);
    }

    public static SerialisableVector3 From(Vector3 vector3) {
        return new SerialisableVector3(vector3.z, vector3.y, vector3.x);
    }
}
}