using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace YakuzaSaveEditor;

static class JsonNodeExt
{
    public static int Gvi(this JsonNode node)
    {
        try { return node.GetValue<int>(); }
        catch
        {
            try { var l = node.GetValue<long>(); return l > int.MaxValue ? int.MaxValue : l < int.MinValue ? int.MinValue : (int)l; }
            catch { return 0; }
        }
    }
    public static long Gvl(this JsonNode node)
    {
        try { return node.GetValue<long>(); }
        catch { try { return (long)node.GetValue<ulong>(); } catch { return 0; } }
    }
    public static bool Gvb(this JsonNode node)
    {
        try { return node.GetValue<bool>(); }
        catch { return false; }
    }
}

public class SaveData
{
    static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = false };

    public JsonNode Root { get; private set; } = null!;
    public string FilePath { get; private set; } = "";
    int _originalSize;

    public static readonly string[] CharacterNames =
    [
        "Ichiban Kasuga", "Yu Nanba", "Koichi Adachi", "Saeko Mukoda",
        "Tianyou Zhao", "Joon-gi Han", "Eri Kamataki", "Masumi Arakawa",
        "Party Member 8", "Party Member 9", "Party Member 10", "Party Member 11"
    ];

    public static readonly string[] JobNames =
    [
        "Hero", "Bodyguard", "Host", "Homeless Guy",
        "Musician", "Foreman", "Chef", "Dealer"
    ];

    public static readonly string[] FemaleJobNames =
    [
        "Heroine", "Matriarch", "Hostess", "Idol",
        "Night Queen", "Barmaid", "Chef", "Dealer"
    ];

    public static readonly Dictionary<int, string> PointIndexNames = new()
    {
        [1] = "Money (¥)",
        [17] = "Casino Coins",
        [21] = "Medals",
        [31] = "Sotenbori Battle Arena Coins",
        [50] = "Battle Arena Coins",
        [51] = "Survival Can Coins",
        [330] = "Management Funds (¥)",
        [332] = "Mgmt: Confidence",
        [333] = "Mgmt: Gutsiness",
        [334] = "Mgmt: Brains",
        [346] = "Mgmt Stat A",
        [347] = "Mgmt Stat B",
        [348] = "Mgmt Stat C",
        [355] = "Mgmt: Capital",
        [356] = "Mgmt: Transfer to Player",
        [370] = "Mgmt: Total Properties",
        [387] = "Mgmt: Revenue",
        [390] = "Mgmt: Period",
        [391] = "Mgmt: Company Rank",
    };

    string _originalJson = "";

    public void Load(string path)
    {
        FilePath = path;
        var bytes = File.ReadAllBytes(path);
        _originalSize = bytes.Length;
        _originalJson = Encoding.UTF8.GetString(bytes);
        Root = JsonNode.Parse(_originalJson, documentOptions: new() { CommentHandling = JsonCommentHandling.Skip })!;
    }

    public void Save(string path)
    {
        var json = Root.ToJsonString(WriteOptions);
        var jsonBytes = Encoding.UTF8.GetBytes(json);
        int diff = _originalSize - jsonBytes.Length;

        if (diff == 0)
        {
            File.WriteAllBytes(path, jsonBytes);
            return;
        }

        if (diff > 0)
        {
            int pos = json.LastIndexOf('}');
            var sb = new StringBuilder(json);
            sb.Insert(pos >= 0 ? pos : json.Length, new string(' ', diff));
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(sb.ToString()));
        }
        else
        {
            int excess = -diff;
            var result = AbsorbExcess(json, excess);
            var resultBytes = Encoding.UTF8.GetBytes(result);
            if (resultBytes.Length < _originalSize)
            {
                int pad = _originalSize - resultBytes.Length;
                int pos = result.LastIndexOf('}');
                var sb = new StringBuilder(result);
                sb.Insert(pos >= 0 ? pos : result.Length, new string(' ', pad));
                File.WriteAllBytes(path, Encoding.UTF8.GetBytes(sb.ToString()));
            }
            else if (resultBytes.Length > _originalSize)
            {
                File.WriteAllBytes(path, resultBytes);
            }
            else
            {
                File.WriteAllBytes(path, resultBytes);
            }
        }
    }

    string AbsorbExcess(string json, int excess)
    {
        var detailKey = "\"m_detail\":\"";
        int idx = json.IndexOf(detailKey);
        if (idx < 0) return json;
        idx += detailKey.Length;

        int endIdx = idx;
        while (endIdx < json.Length)
        {
            if (json[endIdx] == '"' && json[endIdx - 1] != '\\') break;
            endIdx++;
        }

        string detailValue = json[idx..endIdx];

        int trailingSpaces = 0;
        for (int i = detailValue.Length - 1; i >= 0; i--)
        {
            if (detailValue[i] == ' ') trailingSpaces++;
            else break;
        }

        if (trailingSpaces >= excess)
        {
            string newDetail = detailValue[..^excess];
            return json[..idx] + newDetail + json[endIdx..];
        }

        string trimmed = detailValue.TrimEnd();
        string shortened = trimmed;
        int remaining = excess - trailingSpaces;

        while (remaining > 0 && shortened.Contains("\\n"))
        {
            int lastNl = shortened.LastIndexOf("\\n");
            shortened = shortened[..lastNl];
            int saved = (trimmed.Length - shortened.Length) - (excess - remaining);
            if (saved >= 0) remaining = excess - trailingSpaces - (trimmed.Length - shortened.Length);
        }

        return json[..idx] + shortened + json[endIdx..];
    }

    public string GetTitle() => Root["base"]?["m_title"]?.GetValue<string>() ?? "Unknown";
    public void SetTitle(string v) { if (Root["base"] is JsonNode n) n["m_title"] = v; }
    public string GetSubtitle() => Root["base"]?["m_subtitle"]?.GetValue<string>() ?? "";
    public void SetSubtitle(string v) { if (Root["base"] is JsonNode n) n["m_subtitle"] = v; }

    public string? GetIconPath()
    {
        var dir = Path.GetDirectoryName(FilePath);
        if (dir == null) return null;
        var iconPath = Path.Combine(dir, "sce_sys", "icon0.png");
        return File.Exists(iconPath) ? iconPath : null;
    }

    public string? GetSceSysDir()
    {
        var dir = Path.GetDirectoryName(FilePath);
        if (dir == null) return null;
        var sceDir = Path.Combine(dir, "sce_sys");
        return Directory.Exists(sceDir) ? sceDir : null;
    }

    public string? GetParamSfoPath()
    {
        var sceDir = GetSceSysDir();
        if (sceDir == null) return null;
        var p = Path.Combine(sceDir, "param.sfo");
        return File.Exists(p) ? p : null;
    }

    public static string? ReadSfoString(string sfoPath, string key)
    {
        var data = File.ReadAllBytes(sfoPath);
        if (data.Length < 20) return null;
        int keyTableOff = BitConverter.ToInt32(data, 8);
        int dataTableOff = BitConverter.ToInt32(data, 12);
        int numEntries = BitConverter.ToInt32(data, 16);

        for (int i = 0; i < numEntries; i++)
        {
            int entryOff = 20 + i * 16;
            int keyOff = BitConverter.ToUInt16(data, entryOff);
            int paramLen = BitConverter.ToInt32(data, entryOff + 4);
            int dataOff = BitConverter.ToInt32(data, entryOff + 12);

            int absKeyOff = keyTableOff + keyOff;
            int end = absKeyOff;
            while (end < data.Length && data[end] != 0) end++;
            string k = Encoding.UTF8.GetString(data, absKeyOff, end - absKeyOff);

            if (k == key)
            {
                int absDataOff = dataTableOff + dataOff;
                int strLen = paramLen > 0 ? paramLen - 1 : 0;
                return Encoding.UTF8.GetString(data, absDataOff, strLen);
            }
        }
        return null;
    }

    public static bool WriteSfoString(string sfoPath, string key, string value)
    {
        var data = File.ReadAllBytes(sfoPath);
        if (data.Length < 20) return false;
        int keyTableOff = BitConverter.ToInt32(data, 8);
        int dataTableOff = BitConverter.ToInt32(data, 12);
        int numEntries = BitConverter.ToInt32(data, 16);

        for (int i = 0; i < numEntries; i++)
        {
            int entryOff = 20 + i * 16;
            int keyOff = BitConverter.ToUInt16(data, entryOff);
            int paramMaxLen = BitConverter.ToInt32(data, entryOff + 8);
            int dataOff = BitConverter.ToInt32(data, entryOff + 12);

            int absKeyOff = keyTableOff + keyOff;
            int end = absKeyOff;
            while (end < data.Length && data[end] != 0) end++;
            string k = Encoding.UTF8.GetString(data, absKeyOff, end - absKeyOff);

            if (k == key)
            {
                int absDataOff = dataTableOff + dataOff;
                var valBytes = Encoding.UTF8.GetBytes(value);
                int newLen = valBytes.Length + 1;
                if (newLen > paramMaxLen) newLen = paramMaxLen;

                Array.Clear(data, absDataOff, paramMaxLen);
                int copyLen = Math.Min(valBytes.Length, paramMaxLen - 1);
                Array.Copy(valBytes, 0, data, absDataOff, copyLen);

                BitConverter.GetBytes(newLen).CopyTo(data, entryOff + 4);
                File.WriteAllBytes(sfoPath, data);
                return true;
            }
        }
        return false;
    }
    public long GetPlayTick() => Root["base"]?["m_play_tick"]?.Gvl() ?? 0;
    public void SetPlayTick(long v) { if (Root["base"] is JsonNode n) n["m_play_tick"] = v; }
    public int GetDifficulty() => Root["base"]?["m_difficulty"]?["puid"]?.Gvi() ?? 0;
    public void SetDifficulty(int v) { if (Root["base"]?["m_difficulty"] is JsonNode n) n["puid"] = v; }

    public int PointCount => Root["player"]?["point"]?["m_point"]?.AsArray()?.Count ?? 0;
    public long GetPoint(int i) { var a = Root["player"]?["point"]?["m_point"]?.AsArray(); return (a != null && i < a.Count) ? a[i]?.Gvl() ?? 0 : 0; }
    public void SetPoint(int i, long v) { var a = Root["player"]?["point"]?["m_point"]?.AsArray(); if (a != null && i < a.Count) a[i] = v; }

    public int CharacterCount => Root["party"]?["m_chara"]?.AsArray()?.Count ?? 0;
    JsonNode? Chara(int i) => Root["party"]?["m_chara"]?.AsArray()?[i];

    public int GetCharLevel(int i) => Chara(i)?["exp"]?["m_level"]?.Gvi() ?? 0;
    public void SetCharLevel(int i, int v) { if (Chara(i)?["exp"] is JsonNode n) n["m_level"] = v; }
    public long GetCharExp(int i) => Chara(i)?["exp"]?["m_exp"]?.Gvl() ?? 0;
    public void SetCharExp(int i, long v) { if (Chara(i)?["exp"] is JsonNode n) n["m_exp"] = v; }
    public int GetCharHp(int i) => Chara(i)?["hp"]?["m_now"]?.Gvi() ?? 0;
    public int GetCharHpMax(int i) => Chara(i)?["hp"]?["m_max"]?.Gvi() ?? 0;
    public void SetCharHp(int i, int now, int max) { var c = Chara(i); if (c?["hp"] is JsonNode n) { n["m_now"] = now; n["m_max"] = max; } }
    public int GetCharHeat(int i) => Chara(i)?["heat"]?["m_now"]?.Gvi() ?? 0;
    public int GetCharHeatMax(int i) => Chara(i)?["heat"]?["m_max"]?.Gvi() ?? 0;
    public void SetCharHeat(int i, int now, int max) { var c = Chara(i); if (c?["heat"] is JsonNode n) { n["m_now"] = now; n["m_max"] = max; } }
    public int GetCharJob(int i) => Chara(i)?["job"]?["m_current"]?["puid"]?.Gvi() ?? 0;
    public void SetCharJob(int i, int v) { if (Chara(i)?["job"]?["m_current"] is JsonNode n) n["puid"] = v; }

    public (int atk, int def, int agi, int dex, int hp, int mp) GetCharBonus(int i)
    {
        var s = Chara(i)?["status_up"];
        if (s == null) return default;
        return (s["m_atk"]?.Gvi() ?? 0, s["m_def"]?.Gvi() ?? 0,
                s["m_agi"]?.Gvi() ?? 0, s["m_dex"]?.Gvi() ?? 0,
                s["m_hp"]?.Gvi() ?? 0, s["m_mp"]?.Gvi() ?? 0);
    }
    public void SetCharBonus(int i, int atk, int def, int agi, int dex, int hp, int mp)
    {
        var s = Chara(i)?["status_up"]; if (s == null) return;
        s["m_atk"] = atk; s["m_def"] = def; s["m_agi"] = agi; s["m_dex"] = dex; s["m_hp"] = hp; s["m_mp"] = mp;
    }

    public int JobEntryCount => Root["party"]?["m_learn_job"]?["m_list"]?.AsArray()?.Count ?? 0;

    public DataTable GetCharactersTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Level", typeof(int));
        dt.Columns.Add("EXP", typeof(long));
        dt.Columns.Add("HP", typeof(int));
        dt.Columns.Add("HP Max", typeof(int));
        dt.Columns.Add("Heat", typeof(int));
        dt.Columns.Add("Heat Max", typeof(int));
        dt.Columns.Add("Bonus ATK", typeof(int));
        dt.Columns.Add("Bonus DEF", typeof(int));
        dt.Columns.Add("Bonus AGI", typeof(int));
        dt.Columns.Add("Bonus DEX", typeof(int));
        dt.Columns.Add("Bonus HP", typeof(int));
        dt.Columns.Add("Bonus MP", typeof(int));
        dt.Columns.Add("_Idx", typeof(int));
        for (int i = 0; i < CharacterCount && i < CharacterNames.Length; i++)
        {
            var b = GetCharBonus(i);
            dt.Rows.Add(CharacterNames[i], GetCharLevel(i), GetCharExp(i),
                GetCharHp(i), GetCharHpMax(i), GetCharHeat(i), GetCharHeatMax(i),
                b.atk, b.def, b.agi, b.dex, b.hp, b.mp, i);
        }
        return dt;
    }

    public void ApplyCharactersTable(DataTable dt)
    {
        foreach (DataRow row in dt.Rows)
        {
            int i = (int)row["_Idx"];
            SetCharLevel(i, (int)row["Level"]);
            SetCharExp(i, (long)row["EXP"]);
            SetCharHp(i, (int)row["HP"], (int)row["HP Max"]);
            SetCharHeat(i, (int)row["Heat"], (int)row["Heat Max"]);
            SetCharBonus(i, (int)row["Bonus ATK"], (int)row["Bonus DEF"],
                (int)row["Bonus AGI"], (int)row["Bonus DEX"],
                (int)row["Bonus HP"], (int)row["Bonus MP"]);
        }
    }

    public DataTable GetInventoryTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("Slot", typeof(int));
        dt.Columns.Add("Item ID", typeof(int));
        dt.Columns.Add("Count", typeof(int));
        var items = Root["player"]?["carry_item"]?["m_list"]?.AsArray();
        if (items == null) return dt;
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            dt.Rows.Add(i, it?["m_item_id"]?["puid"]?.Gvi() ?? 0, it?["m_count"]?.Gvi() ?? 0);
        }
        return dt;
    }
    public void ApplyInventoryTable(DataTable dt)
    {
        var items = Root["player"]?["carry_item"]?["m_list"]?.AsArray(); if (items == null) return;
        foreach (DataRow row in dt.Rows)
        {
            int slot = (int)row["Slot"];
            if (slot < items.Count)
            {
                items[slot]!["m_item_id"]!["puid"] = (int)row["Item ID"];
                items[slot]!["m_count"] = (int)row["Count"];
            }
        }
    }

    public DataTable GetBoxItemTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("Slot", typeof(int));
        dt.Columns.Add("Item ID", typeof(int));
        dt.Columns.Add("Count", typeof(int));
        var items = Root["player"]?["box_Item"]?["m_list"]?.AsArray();
        if (items == null) return dt;
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            dt.Rows.Add(i, it?["m_item_id"]?["puid"]?.Gvi() ?? 0, it?["m_count"]?.Gvi() ?? 0);
        }
        return dt;
    }
    public void ApplyBoxItemTable(DataTable dt)
    {
        var items = Root["player"]?["box_Item"]?["m_list"]?.AsArray(); if (items == null) return;
        foreach (DataRow row in dt.Rows)
        {
            int slot = (int)row["Slot"];
            if (slot < items.Count)
            {
                items[slot]!["m_item_id"]!["puid"] = (int)row["Item ID"];
                items[slot]!["m_count"] = (int)row["Count"];
            }
        }
    }

    public DataTable GetPointsTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("Index", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Value", typeof(long));
        dt.Columns.Add("Total Added", typeof(long));
        dt.Columns.Add("Total Subtracted", typeof(long));
        var pts = Root["player"]?["point"]?["m_point"]?.AsArray();
        var add = Root["player"]?["point"]?["m_point_total_add"]?.AsArray();
        var sub = Root["player"]?["point"]?["m_point_total_sub"]?.AsArray();
        if (pts == null) return dt;
        for (int i = 0; i < pts.Count; i++)
        {
            long v = pts[i]?.Gvl() ?? 0;
            long a = add != null && i < add.Count ? add[i]?.Gvl() ?? 0 : 0;
            long s = sub != null && i < sub.Count ? sub[i]?.Gvl() ?? 0 : 0;
            PointIndexNames.TryGetValue(i, out var name);
            dt.Rows.Add(i, name ?? "", v, a, s);
        }
        return dt;
    }
    public void ApplyPointsTable(DataTable dt)
    {
        var pts = Root["player"]?["point"]?["m_point"]?.AsArray();
        if (pts == null) return;
        foreach (DataRow row in dt.Rows)
        {
            int idx = (int)row["Index"];
            if (idx < pts.Count) pts[idx] = (long)row["Value"];
        }
    }

    public DataTable GetJobsTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("Character", typeof(string));
        dt.Columns.Add("Job", typeof(string));
        dt.Columns.Add("Level", typeof(int));
        dt.Columns.Add("EXP", typeof(long));
        dt.Columns.Add("_CharIdx", typeof(int));
        dt.Columns.Add("_JobIdx", typeof(int));
        var list = Root["party"]?["m_learn_job"]?["m_list"]?.AsArray();
        if (list == null) return dt;
        int jobsPerChar = list.Count / Math.Max(1, CharacterCount);
        for (int c = 0; c < CharacterCount; c++)
        {
            string charName = c < CharacterNames.Length ? CharacterNames[c] : $"Character {c}";
            bool isFemale = c == 3 || c == 6;
            var names = isFemale ? FemaleJobNames : JobNames;
            for (int j = 0; j < jobsPerChar && (c * jobsPerChar + j) < list.Count; j++)
            {
                int idx = c * jobsPerChar + j;
                var entry = list[idx];
                string jobName = j < names.Length ? names[j] : $"Job {j}";
                dt.Rows.Add(charName, jobName,
                    entry?["m_level"]?.Gvi() ?? 0,
                    entry?["m_nExp"]?.Gvl() ?? 0,
                    c, j);
            }
        }
        return dt;
    }
    public void ApplyJobsTable(DataTable dt)
    {
        var list = Root["party"]?["m_learn_job"]?["m_list"]?.AsArray();
        if (list == null) return;
        int jobsPerChar = list.Count / Math.Max(1, CharacterCount);
        foreach (DataRow row in dt.Rows)
        {
            int c = (int)row["_CharIdx"], j = (int)row["_JobIdx"];
            int idx = c * jobsPerChar + j;
            if (idx < list.Count)
            {
                list[idx]!["m_level"] = (int)row["Level"];
                list[idx]!["m_nExp"] = (long)row["EXP"];
            }
        }
    }

    JsonNode? Company => Root["mg_upstart"]?["m_company"];
    public int GetCompanyMaxRank() => Company?["m_n_max_rank"]?.Gvi() ?? 0;
    public void SetCompanyMaxRank(int v) { if (Company is JsonNode n) n["m_n_max_rank"] = v; }
    public int GetCompanyTargetRank() => Company?["m_n_target_rank"]?.Gvi() ?? 0;
    public void SetCompanyTargetRank(int v) { if (Company is JsonNode n) n["m_n_target_rank"] = v; }
    public int GetCompanyTurnCount() => Company?["m_n_turn_cnt"]?.Gvi() ?? 0;
    public void SetCompanyTurnCount(int v) { if (Company is JsonNode n) n["m_n_turn_cnt"] = v; }
    public int GetCompanyPeriod() => Company?["m_n_period_num"]?.Gvi() ?? 0;
    public void SetCompanyPeriod(int v) { if (Company is JsonNode n) n["m_n_period_num"] = v; }
    public int GetCompanyStockIdx() => Company?["m_n_stock_price_idx"]?.Gvi() ?? 0;
    public void SetCompanyStockIdx(int v) { if (Company is JsonNode n) n["m_n_stock_price_idx"] = v; }
    public int GetCompanyScale() => Company?["m_scale_id"]?["puid"]?.Gvi() ?? 0;
    public void SetCompanyScale(int v) { if (Company?["m_scale_id"] is JsonNode n) n["puid"] = v; }

    public int GetSceneId() => Root["scene"]?["m_scene_id"]?.Gvi() ?? 0;
    public void SetSceneId(int v) { if (Root["scene"] is JsonNode n) n["m_scene_id"] = v; }
    public int GetSceneConfigId() => Root["scene"]?["m_scene_config_id"]?.Gvi() ?? 0;
    public void SetSceneConfigId(int v) { if (Root["scene"] is JsonNode n) n["m_scene_config_id"] = v; }
    public int GetStage() => Root["scene"]?["m_stage"]?.Gvi() ?? 0;
    public void SetStage(int v) { if (Root["scene"] is JsonNode n) n["m_stage"] = v; }
    public int GetDayNight() => Root["scene"]?["m_daynight"]?.Gvi() ?? 0;
    public void SetDayNight(int v) { if (Root["scene"] is JsonNode n) n["m_daynight"] = v; }

    public float GetStomachNow() => (float)(Root["player"]?["stomach"]?["m_now"]?.GetValue<double>() ?? 0);
    public float GetStomachMax() => (float)(Root["player"]?["stomach"]?["m_max"]?.GetValue<double>() ?? 0);
    public void SetStomach(float now, float max) { var s = Root["player"]?["stomach"]; if (s != null) { s["m_now"] = now; s["m_max"] = max; } }
    public int GetClothing() => Root["player"]?["clothing"]?["m_now"]?["puid"]?.Gvi() ?? 0;
    public void SetClothing(int v) { if (Root["player"]?["clothing"]?["m_now"] is JsonNode n) n["puid"] = v; }
    public int GetDrunkMaxLevel() => Root["player"]?["drunk2"]?["m_max_level"]?.Gvi() ?? 0;
    public void SetDrunkMaxLevel(int v) { if (Root["player"]?["drunk2"] is JsonNode n) n["m_max_level"] = v; }
    public int GetBattleStyle() => Root["player"]?["battle_style"]?["m_now"]?["puid"]?.Gvi() ?? 0;
    public void SetBattleStyle(int v) { if (Root["player"]?["battle_style"]?["m_now"] is JsonNode n) n["puid"] = v; }

    public Dictionary<string, object> GetMinigameScalars(string section)
    {
        var result = new Dictionary<string, object>();
        var node = Root[section];
        if (node == null) return result;
        CollectScalars(node, "", result);
        return result;
    }

    void CollectScalars(JsonNode node, string prefix, Dictionary<string, object> result)
    {
        if (node is JsonObject obj)
        {
            foreach (var kv in obj)
            {
                if (kv.Key == ".ver") continue;
                string path = string.IsNullOrEmpty(prefix) ? kv.Key : $"{prefix}.{kv.Key}";
                if (kv.Value is JsonObject child)
                    CollectScalars(child, path, result);
                else if (kv.Value is JsonArray)
                    continue;
                else if (kv.Value != null)
                {
                    try
                    {
                        var v = kv.Value.GetValue<JsonElement>();
                        if (v.ValueKind == JsonValueKind.Number)
                        {
                            if (v.TryGetInt64(out long lv)) result[path] = lv;
                            else if (v.TryGetUInt64(out ulong uv)) result[path] = (long)uv;
                            else if (v.TryGetDouble(out double dv)) result[path] = dv;
                        }
                        else if (v.ValueKind == JsonValueKind.True || v.ValueKind == JsonValueKind.False)
                            result[path] = v.GetBoolean();
                        else if (v.ValueKind == JsonValueKind.String)
                            result[path] = v.GetString()!;
                    }
                    catch { }
                }
            }
        }
    }

    public void SetMinigameScalar(string section, string path, object value)
    {
        var node = Root[section];
        if (node == null) return;
        var parts = path.Split('.');
        for (int i = 0; i < parts.Length - 1; i++)
        {
            node = node[parts[i]];
            if (node == null) return;
        }
        string last = parts[^1];
        if (value is long lv) node[last] = lv;
        else if (value is int iv) node[last] = iv;
        else if (value is double dv) node[last] = dv;
        else if (value is bool bv) node[last] = bv;
        else if (value is string sv) node[last] = sv;
    }

    public DataTable GetKaraokeTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("Index", typeof(int));
        dt.Columns.Add("Max Point", typeof(int));
        dt.Columns.Add("Max Rank", typeof(int));
        dt.Columns.Add("Play Count", typeof(int));
        dt.Columns.Add("Flags", typeof(int));
        var list = Root["mg_karaoke"]?["m_song_detail"]?.AsArray();
        if (list == null) return dt;
        for (int i = 0; i < list.Count; i++)
        {
            var s = list[i];
            dt.Rows.Add(i, s?["max_point"]?.Gvi() ?? 0, s?["max_rank"]?.Gvi() ?? 0,
                s?["play_cnt"]?.Gvi() ?? 0, s?["flags"]?.Gvi() ?? 0);
        }
        return dt;
    }
    public void ApplyKaraokeTable(DataTable dt)
    {
        var list = Root["mg_karaoke"]?["m_song_detail"]?.AsArray();
        if (list == null) return;
        foreach (DataRow row in dt.Rows)
        {
            int idx = (int)row["Index"];
            if (idx >= list.Count) continue;
            list[idx]!["max_point"] = (int)row["Max Point"];
            list[idx]!["max_rank"] = (int)row["Max Rank"];
            list[idx]!["play_cnt"] = (int)row["Play Count"];
            list[idx]!["flags"] = (int)row["Flags"];
        }
    }

    public DataTable GetDragonCartTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("Index", typeof(int));
        dt.Columns.Add("Drive Perf", typeof(int));
        dt.Columns.Add("HP Level", typeof(int));
        dt.Columns.Add("Item Level", typeof(int));
        dt.Columns.Add("Ring Level", typeof(int));
        var list = Root["mg_dragon_cart"]?["m_cart_param_list"]?.AsArray();
        if (list == null) return dt;
        for (int i = 0; i < list.Count; i++)
        {
            var c = list[i];
            dt.Rows.Add(i,
                c?["m_upgrade_level_drive_performance"]?.Gvi() ?? 0,
                c?["m_upgrade_level_hp"]?.Gvi() ?? 0,
                c?["m_upgrade_level_item"]?.Gvi() ?? 0,
                c?["m_upgrade_level_ring"]?.Gvi() ?? 0);
        }
        return dt;
    }
    public void ApplyDragonCartTable(DataTable dt)
    {
        var list = Root["mg_dragon_cart"]?["m_cart_param_list"]?.AsArray();
        if (list == null) return;
        foreach (DataRow row in dt.Rows)
        {
            int idx = (int)row["Index"];
            if (idx >= list.Count) continue;
            list[idx]!["m_upgrade_level_drive_performance"] = (int)row["Drive Perf"];
            list[idx]!["m_upgrade_level_hp"] = (int)row["HP Level"];
            list[idx]!["m_upgrade_level_item"] = (int)row["Item Level"];
            list[idx]!["m_upgrade_level_ring"] = (int)row["Ring Level"];
        }
    }

    public DataTable GetMinigameTable(string section)
    {
        var dt = new DataTable();
        dt.Columns.Add("Property", typeof(string));
        dt.Columns.Add("Value", typeof(string));
        dt.Columns.Add("_Type", typeof(string));
        var scalars = GetMinigameScalars(section);
        foreach (var kv in scalars)
        {
            string type = kv.Value switch { bool => "bool", long => "long", double => "double", _ => "string" };
            dt.Rows.Add(kv.Key, kv.Value.ToString(), type);
        }
        return dt;
    }

    public void ApplyMinigameTable(string section, DataTable dt)
    {
        foreach (DataRow row in dt.Rows)
        {
            string prop = (string)row["Property"];
            string val = (string)row["Value"];
            string type = (string)row["_Type"];
            object parsed = type switch
            {
                "bool" => bool.TryParse(val, out var b) && b,
                "long" => long.TryParse(val, out var l) ? l : 0L,
                "double" => double.TryParse(val, out var d) ? d : 0.0,
                _ => val
            };
            SetMinigameScalar(section, prop, parsed);
        }
    }
}
