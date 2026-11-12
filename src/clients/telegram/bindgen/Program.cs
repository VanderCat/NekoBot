using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace NekoBot.Telegram.Bindgen;

public class Program {
    public static async Task Main(string[] args) {
        var layerlist = new Dictionary<int, string> {
            { 2, "New userpic notifications" },
            { 3, "Send message can trigger link change" },
            { 4, "Check-in chats" },
            { 5, "Localized SMS, localized notifications" },
            { 6, "Foursquare integration" },
            { 7, "Added wallPaperSolid" },
            { 8, "Added end-to-end encryption" },
            { 9, "Improved big files upload perfomance" },
            { 10, "Improved chat participants updates" },
            { 11, "Improved secret chats" },
            { 12, "New dynamic support" },
            { 13, "Audio, video MIME; contacts import retry; new secret actions" },
            { 14, "Notify settings sync, blacklist sync" },
            { 15, "Modified getHistory offset behaviour" },
            { 16, "Split sendCode into 2 parts" },
            { 17, "Added custom typing, introduced message flags" },
            { 18, "Added usernames" },
            { 23, "Stickers for secret chats" },
            { 105, "Scheduled messages, Cloud themes" },
            { 108, "Login with QR code" },
            { 109, "Polls v2" },
            { 110, "People Nearby 2.0, Bank card entity" },
            { 111, "Folders, Broadcast Stats" },
            { 112, "Old featured stickers, generic dice, poll timer, poll solution" },
            { 113, "PSA" },
            { 114, "Video thumbs for GIFs" },
            { 115, "Peek Channel Invite" },
            { 116, "Group Stats, Profile Videos" },
            { 117, "WebRTC Phone Calls" },
            { 118, "Callback with 2FA, Countries list" },
            { 119, "Comments in channels, Threads, Anonymous Admins" },
            { 120, "Multipins, Message Stats, GeoLive v2" },
            { 121, "SVG-based Outlines for Stickers" },
            { 122, "Voice Chats" },
            { 123, "Voice Chat improvements" },
            { 124, "Expiring Invite links" },
            { 125, "Voice Chats in Broadcasts" },
            { 126, "Ban channels in channels" },
            { 127, "Payments in channels" },
            { 128, "Microthumbs for User/Chat profile photos" },
            { 129, "Video Chats" },
            { 130, "Custom placeholder for bot reply keyboards" },
            { 131, "Reset 2FA Password after a week" },
            { 132, "Chat themes" },
            { 133, "64-bit IDs for User/Chat" },
            { 134, "Chat Requests, Shared Media Calendar" },
            { 135, "Send Message As a Channel" },
            { 136, "Reactions" },
            { 137, "Translations" },
            { 138, "GIF Sticker Packs" },
            { 139, "RTMP streaming" },
            { 140, "WebApps, Cloud Ringtones" },
            { 142, "TCP Reflectors" },
            { 143, "Premium Subscription, Cloud Invoices" },
            { 144, "Premium as a Gift, Custom Emoji" },
            { 145, "Custom Reactions, Statuses, Sign In with email" },
            { 147, "Keywords for stickers and emojis" },
            { 148, "Forums, collectible usernames" },
            { 150, "Pinned forum topics, general topic" },
            { 151, "Media spoilers, suggested profile photos" },
            { 152, "Real-time translations, Firebase SMS authentication" },
            { 153, "Modify created stickersets" },
            { 155, "Dates for reactions" },
            { 158, "Shared folders, per-chat wallpapers" },
            { 159, "Anonymous votes" },
            { 160, "Stories" },
            { 164, "Stories in Channels" },
            { 166, "Giveaways in channels" },
            { 167, "Similar channels" },
            { 168, "Channel colors" },
            { 169, "Multiselection of chats for bots" },
            { 170, "Saved Messages 2.0" },
            { 171, "Saved Messages 2.0" },
            { 174, "Group boosts" },
            { 176, "Business" },
            { 177, "Business Bots, Birthdays" },
            { 178, "Saved Personal channel, Reactions notifications" },
            { 179, "Channel revenue stats, phrases in SMS" },
            { 180, "Message Effects, Hashtags" },
            { 181, "Stars" },
            { 182, "Stars Revenue" },
            { 183, "Paid posts" },
            { 184, "Stars Refunds" },
            { 185, "MiniApp Store, Star Gifts" },
            { 186, "Channel Subscriptions for Stars" },
            { 187, "Stars Giveaways" },
            { 189, "Stars Gifts" },
            { 192, "Video Qualities, Ads in bots" },
            { 194, "Stars Subscription for Bots" },
            { 195, "Affiliate Programs for Bots" },
            { 196, "Collectible gifts" },
            { 197, "Similar Bots" },
            { 198, "Collectibles as emoji statuses" },
            { 200, "Paid Messages" },
            { 202, "Conference calls" },
            { 203, "Resell collectible gifts" },
            { 204, "Monoforums" },
            { 205, "TODO lists" },
            { 207, "Suggested channel posts" },
            { 210, "Star gift collections, rating" },
            { 211, "Story Albums" },
            { 212, "Prepaid gift upgrades" },
            { 213, "Music in profile" },
            { 214, "Chat Themes with collectible gifts" },
            { 215, "Chat Gift Themes" },
            { 216, "Threaded view for Bots" },
            { 217, "Live Stories" },
            { 218, "Gift Auctions" },
            { 219, "Passkeys" },
            { 220, "Gift Offers" },
            { 222, "Gift Crafts" },
            { 223, "User tags in groups" },
            { 224, "AI Editor, Live photos" },
            { 225, "Guest mode, Custom AI compose tones" },
        };
        
        var handler = new HttpClientHandler {
            Proxy = new WebProxy(new Uri("socks5://localhost:2080"))
        };
        var resolver = new TlSchemaResolver(handler);
        var docResolver = new TlDocumentationResolver(handler);
        var layerResolved = new ConcurrentDictionary<int, TLSchemaModel>();
        var resolveTasks = new List<Task>();
        foreach (var layer in layerlist) {
            resolveTasks.Add(Task.Run(async () => {
                layerResolved[layer.Key] = await resolver.FetchLayerAsync(layer.Key);
                Console.WriteLine($"downloaded layer {layer.Key} {layer.Value}");
            }));
        }
        Task.WaitAll(resolveTasks);
        Console.WriteLine($"all layers downloaded, generating");
        var sortedLayers = layerlist.Keys.OrderBy(i => i);
        // var first = sortedLayers[0];
        // sortedLayers.RemoveAt(0);
        // var intialLayer = layerResolved[first];
        var schema = new GeneratedSchema();
        // foreach (var constructor in intialLayer.Constructors) {
        //     if (!types.TryGetValue(constructor.Type, out var type)) {
        //         type = types[constructor.Type] = new GeneratedType();
        //     }
        //     var docs = await docResolver.GetDocumentationForConstructorAsync(constructor.Predicate, first);
        //     type.Constructors[$"{constructor.Predicate}#{constructor:x}"] = new GeneratedConstructor {
        //         Name = constructor.Predicate,
        //         Id = constructor.Id,
        //         Remarks = docs?.Remarks ?? "",
        //         Summary = docs?.Summary ?? "",
        //         SinceLayer = first,
        //         UntilLayer = first + 1,
        //     };
        // }
        foreach (var layer in sortedLayers) {
            foreach (var constructor in layerResolved[layer].Constructors) {
                TlDocumentationModel? typeDocs = null;//await docResolver.GetDocumentationForTypeAsync(constructor.Predicate, layer);
                if (!schema.Types.TryGetValue(constructor.Type, out var type)) {
                    type = schema.Types[constructor.Type] = new GeneratedType {
                        Name = constructor.Type,
                        Summary = typeDocs?.Summary??"",
                        Remarks = typeDocs?.Remarks??"",
                        SinceLayer = layer,
                        UntilLayer = layer+1,
                    };
                }
                else {
                    type.UntilLayer = layer + 1;
                }
                var constructorId = $"{constructor.Predicate}#{constructor.Id:x}";
                if (type.Constructors.TryGetValue(constructorId, out var ctorExisting)) {
                    ctorExisting.UntilLayer = layer+1;
                    continue;
                }

                TlDocumentationModel? docs = null;//await docResolver.GetDocumentationForConstructorAsync(constructor.Predicate, layer);
                var genFn = type.Constructors[constructorId] = new GeneratedFunction {
                    Name = constructor.Predicate,
                    Id = constructor.Id,
                    Remarks = docs?.Remarks ?? "",
                    Summary = docs?.Summary ?? "",
                    SinceLayer = layer,
                    UntilLayer = layer + 1,
                };
                foreach (var param in constructor.Params) {
                    genFn.Parameters.Add(new GeneratedParameter {
                        Name = param.Name,
                        Type = param.Type,
                        Description = "", //TODO: description
                    });
                }
            }

            foreach (var function in layerResolved[layer].Methods) {
                var functionId = $"{function.Method}#{function.Id:x}";
                if (schema.Functions.TryGetValue(functionId, out var ctorExisting)) {
                    ctorExisting.UntilLayer = layer+1;
                    continue;
                }
                TlDocumentationModel? fnDocs = null;//await docResolver.GetDocumentationForMethodAsync(function.Predicate, layer);
                var genFn = schema.Functions[functionId] = new GeneratedFunction {
                    Name = function.Method,
                    Id = function.Id,
                    Remarks = fnDocs?.Remarks ?? "",
                    Summary = fnDocs?.Summary ?? "",
                    SinceLayer = layer,
                    UntilLayer = layer + 1,
                };
                foreach (var param in function.Params) {
                    genFn.Parameters.Add(new GeneratedParameter {
                        Name = param.Name,
                        Type = param.Type,
                        Description = "", //TODO: description
                    });
                }
            }
        }

        await JsonSerializer.SerializeAsync(File.Open("schema.json", FileMode.OpenOrCreate, FileAccess.Write), schema, new JsonSerializerOptions(JsonSerializerDefaults.Web) {
            WriteIndented = true
        });
        
        //
        // var layer2 = await resolver.FetchLayerAsync(28);
        // var constructorDiff = new Dictionary<string, TLSchemaConstructorModel>();
        // foreach (var constructor in result.Constructors) {
        //     constructorDiff[constructor.Predicate] = constructor;
        // }
        // var present = new List<string>();
        // var changed = new List<string>();
        // foreach (var constructor in layer2.Constructors) {
        //     if (constructorDiff.TryGetValue(constructor.Predicate, out var oldConstructor)) {
        //         present.Add(oldConstructor.Predicate);
        //         if (oldConstructor.Id != constructor.Id) changed.Add(oldConstructor.Predicate);
        //         continue;
        //     }
        //     Console.WriteLine($"Added: {constructor.Predicate}");
        // }
        //
        // foreach (var constructor in result.Constructors) {
        //     if (!present.Contains(constructor.Predicate)) {
        //         Console.WriteLine($"Removed: {constructor.Predicate}");
        //     }
        // }
        //
        // foreach (var change in changed) {
        //     Console.WriteLine($"Changed: {change}");
        // }
        // if (result is not null) {
        //     foreach (var constructor in result.Constructors) {
        //         try {
        //             var res = await docResolver.GetDocumentationForConstructorAsync(constructor.Predicate);
        //             Console.WriteLine($"{res.Name}: {res.Summary}");
        //         }
        //         catch (Exception e) {
        //             Console.WriteLine($"error writing {constructor.Predicate}");
        //             Console.WriteLine(e);
        //         }
        //     }
        // }
    }
}