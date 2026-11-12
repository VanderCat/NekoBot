// namespace NekoBot.Telegram;
//
// [TlType]
// public abstract record InputMedia : TlType;
//
// [TelegramId(0x9664f57f)]
// [TelegramConstructor("inputMediaEmpty")]
// public sealed record InputMediaEmpty() : InputMedia {
//     public override void Write(Stream stream) {
//         stream.Write();
//     }
// }
// [TelegramId(0x9664f57f)]
// [TelegramConstructor("inputMediaUploadedPhoto")]
// public sealed record InputMediaUploadedPhoto(bool spoiler, bool live_photo, InputFile file, List<InputDocument> stickers, int ttl_seconds, InputDocument video) : InputMedia;