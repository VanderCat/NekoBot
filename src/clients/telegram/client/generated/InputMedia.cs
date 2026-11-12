// Copyright 2026 VanderCat

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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