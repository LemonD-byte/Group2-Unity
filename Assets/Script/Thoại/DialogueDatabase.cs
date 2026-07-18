using System.Collections.Generic;

/// <summary>
/// Kịch bản thoại 4 màn. Mỗi màn gộp thành 1 hàm duy nhất (không chia nhỏ theo scene)
/// để dễ gọi: DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel1(), callback);
/// Nhân vật: Nam (player), Hoa (vợ), Bin (con).
/// </summary>
public static class DialogueDatabase
{
    // MÀN 1 — Khu phố Bình An | 10 zombie thường | làm quen điều khiển, giữ máu, tính giờ
    public static List<DialogueLine> GetLevel1()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Hệ thống", "Ngày 15 tháng 10. Khu phố Bình An, một buổi sáng như mọi ngày."),
            new DialogueLine("Nam", "Ưm... để xem điện thoại có tin gì không."),
            new DialogueLine("Tin nhắn của Hoa", "Anh ơi, em đưa Bin đi học rồi tranh thủ ghé chợ. Đồ ăn sáng em để trong tủ lạnh, anh hâm lại rồi hẵng ăn nhé."),
            new DialogueLine("Nam", "Xuống bếp kiếm cái gì bỏ bụng đã... Ầm! Tiếng gì thế, hình như bên nhà anh Hùng chị Lan?"),
            new DialogueLine("Nam", "Cầm theo cây gậy với khẩu súng lục phòng thân rồi qua xem thử."),
            new DialogueLine("Nam", "Chị Lan! ...Máu nhiều quá, chị ấy không còn thở nữa. Anh Hùng, da anh sao tái nhợt thế kia?"),
            new DialogueLine("Hùng (Hàng xóm)", "Gừ... gào..."),
            new DialogueLine("Nam", "Anh làm sao vậy?! Tránh xa tôi ra!"),
            new DialogueLine("Nam", "(Hạ gục xong) Tôi chỉ tự vệ thôi mà... sao anh ấy lại cắn xé như thú hoang vậy?"),
            new DialogueLine("Nam", "Ra ngoài xem tình hình... Trời, cả khu phố loạn hết rồi. Phải lấy xe đi tìm Hoa và Bin ngay!")
        };
    }


    public static List<DialogueLine> GetLevel2()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Nam", "Chợ huyện đây rồi... sao hỗn loạn hơn cả khu phố mình vậy? Hoa nhắn là đang ở khu này."),
            new DialogueLine("Nam", "Hoa! Bin! Là hai người đấy phải không?!"),
            new DialogueLine("Bin", "Bố! Con chạy trốn được rồi tìm thấy mẹ ở đây!"),
            new DialogueLine("Hoa", "Nam... đừng lại gần em quá..."),
            new DialogueLine("Nam", "Em bị cắn rồi sao? Nhưng sao vẫn nhận ra bọn anh, vẫn nói chuyện được?"),
            new DialogueLine("Hoa", "Em cũng không hiểu... có cơn thèm khát gì đó giằng xé trong người, nhưng đầu óc vẫn tỉnh táo. Em vẫn nhớ anh, nhớ Bin..."),
            new DialogueLine("Nam", "Dù thế nào anh cũng không bỏ hai mẹ con lại. Mình sẽ tìm cách."),
            new DialogueLine("Đại úy Phong", "Có người sống sót! Khoan đã, người phụ nữ kia nhiễm bệnh rồi! Chuẩn bị vũ khí!"),
            new DialogueLine("Nam", "Đừng bắn! Cô ấy không tấn công ai cả, vẫn còn giữ được lý trí!"),
            new DialogueLine("Tiến sĩ Minh", "Đồng tử cô ấy không hề bị đục như các ca khác... đây là trường hợp chưa từng thấy."),
            new DialogueLine("Đại úy Phong", "Được rồi, theo chúng tôi về trại tị nạn."),
            new DialogueLine("Tiến sĩ Minh", "Vợ anh mang kháng thể đặc biệt, là chìa khóa bào chế thuốc giải. Nhưng nguyên liệu chỉ còn ở Bệnh viện Quân y cũ, nơi đã bị cô lập bởi zombie."),
            new DialogueLine("Nam", "Tôi sẽ đi cùng các anh. Vì Hoa, vì Bin, vì tất cả mọi người.")
        };
    }



    public static List<DialogueLine> GetLevel3_Wave1()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Đại úy Phong", "Bệnh viện quân y phía trước. Tôi giữ cổng, cậu và Tiến sĩ Minh vào tìm nguyên liệu."),
            new DialogueLine("Tiến sĩ Minh", "Cần đủ 3 loại nguyên liệu trong phòng thí nghiệm tầng 3. Cẩn thận, bên trong không hề yên tĩnh."),
            new DialogueLine("Tiến sĩ Minh", "Tủ mẫu vẫn còn nguyên, nhưng tiếng động đã đánh thức đám zombie khu này rồi!"),
            new DialogueLine("Nam", "Cứ để tôi lo, ông tập trung thu thập đi!")
        };
    }

    public static List<DialogueLine> GetLevel3_Wave2()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Tiến sĩ Minh", "Đủ cả 3 loại rồi, rút lui thôi—"),
            new DialogueLine("Nam", "Khoan, nghe không? Một đàn lớn hơn đang kéo tới, chúng vây kín lối ra rồi!")
        };
    }

    public static List<DialogueLine> GetLevel3_End()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Tiến sĩ Minh", "Ra được rồi... suýt nữa thì không kịp."),
            new DialogueLine("Nam", "Về căn cứ ngay, Hoa đang đợi chúng ta.")
        };
    }


    public static List<DialogueLine> GetLevel4_Wave1()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Nam", "Tiến sĩ, nguyên liệu đây, bào chế thuốc giải cho Hoa ngay đi!"),
            new DialogueLine("Tiến sĩ Minh", "Tôi cần khoảng 15 phút. Nhưng chúng ta có vấn đề lớn rồi."),
            new DialogueLine("Đại úy Phong", "Nam! Tiếng động cơ xe đã thu hút cả đàn zombie, chúng đang bao vây căn cứ!"),
            new DialogueLine("Hoa", "Anh cẩn thận, em sẽ đợi anh trong này."),
            new DialogueLine("Nam", "Đợi anh 15 phút thôi, Hoa. Anh sẽ bảo vệ nơi này bằng mọi giá.")
        };
    }

    public static List<DialogueLine> GetLevel4_Wave2()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Đại úy Phong", "Mặt đất đang rung? Cái quái gì đang tới vậy?!"),
            new DialogueLine("Tiến sĩ Minh", "(qua bộ đàm) Cẩn thận! Một thể đột biến cỡ lớn đang tiến về phía các anh!"),
            new DialogueLine("Nam", "Lớn gấp mấy lần zombie thường... đây mới là thử thách thật sự.")
        };
    }

    public static List<DialogueLine> GetLevel4_End()
    {
        return new List<DialogueLine>()
        {
            new DialogueLine("Đại úy Phong", "Hạ được nó rồi... căn cứ vẫn đứng vững."),
            new DialogueLine("Tiến sĩ Minh", "Thuốc giải xong rồi. Nam, đưa Hoa vào đây ngay."),
            new DialogueLine("Hoa", "Em cảm thấy cơn khát đang biến mất rồi..."),
            new DialogueLine("Bin", "Mẹ ơi! Mẹ tỉnh lại thật rồi!"),
            new DialogueLine("Nam", "Mọi chuyện qua rồi. Từ giờ, chúng ta cùng nhau xây dựng lại tất cả.")
        };
    }
}
