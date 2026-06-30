using UnityEngine;

[CreateAssetMenu(fileName = "NewBookData", menuName = "Books/Book Data")]
public class BookData : ScriptableObject
{
    public string bookTitle;
    public string genre; // Crime, Drama, Classic...
    public Color genreColor; // Màu sắc riêng cho từng thể loại
    public Sprite bookCover; // Ảnh bìa sách
}