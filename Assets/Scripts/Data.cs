using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CargoData
{
    public string cargoID;
    public string cargoName;
    public float weight;
    public int price;
}

// Vì JsonUtility không hỗ trợ chuyển đổi trực tiếp một mảng/List ở lớp ngoài cùng, 
// chúng ta cần một class bọc (Wrapper) như dưới đây:
[System.Serializable]
public class CargoListWrapper
{
    public List<CargoData> cargoList;
}