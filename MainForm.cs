using System.Data;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace YakuzaSaveEditor;

public class MainForm : XtraForm
{
    SaveData _save = null!;
    XtraTabControl _tabs = null!;
    LabelControl _welcome = null!;

    SpinEdit _money = null!, _mgmtFunds = null!, _difficulty = null!;
    SpinEdit _playTime = null!;
    SpinEdit _stomachNow = null!, _stomachMax = null!, _clothing = null!, _drunkMax = null!, _battleStyle = null!;
    SpinEdit _compRank = null!, _compTarget = null!, _compTurns = null!, _compPeriod = null!, _compStock = null!, _compScale = null!;
    SpinEdit _sceneId = null!, _sceneConfig = null!, _stage = null!, _dayNight = null!;
    LabelControl _titleLabel = null!;
    TextEdit _saveTitle = null!, _saveSubtitle = null!;
    PictureEdit _iconPreview = null!;
    SimpleButton _iconChangeBtn = null!;
    PanelControl _iconPanel = null!;
    string? _newIconPath;

    GridControl _charGrid = null!, _jobGrid = null!, _invGrid = null!, _boxGrid = null!, _ptsGrid = null!;
    GridControl _karaokeGrid = null!, _cartGrid = null!;
    readonly Dictionary<string, GridControl> _mgGrids = [];

        static readonly Dictionary<int, string> ItemNames = new()
    {
        [3669] = "Staminan X",
        [3670] = "Staminan XX",
        [3671] = "Staminan Royale",
        [3672] = "Staminan Spark",
        [3673] = "Staminan Light",
        [3674] = "Tauriner",
        [3675] = "Tauriner +",
        [3676] = "Tauriner ++",
        [3677] = "Tauriner Maximum",
        [3678] = "Toughness Infinity",
        [3679] = "Toughness Z",
        [3680] = "Toughness ZZ",
        [3682] = "Platinum Plate",
        [3683] = "Gold Plate",
        [3684] = "Silver Plate",
        [3685] = "Bronze Plate",
        [3686] = "Iron Plate",
        [3687] = "Trash",
        [3688] = "Pocket Tissues",
        [3716] = "Bento Lunch Set",
        [3717] = "Chicken Karaage Bento",
        [3718] = "Pork Tonkatsu Bento",
        [3719] = "Luxury Yakiniku Bento",
        [3721] = "Lettuce Sandwich",
        [3722] = "Tuna & Egg Sandwich",
        [3724] = "Club Sandwich",
        [3726] = "Seaweed Onigiri",
        [3727] = "Salmon Onigiri",
        [3728] = "Tuna Onigiri",
        [3732] = "Premium Sushi Set",
        [3738] = "Frankfurter",
        [3741] = "Gold Safe Key",
        [3743] = "Half-eaten Bento",
        [3744] = "Half-empty Drink",
        [3745] = "Disposed Bento",
        [3746] = "Cockroach",
        [3747] = "Soup Kitchen Pork Soup",
        [3749] = "10 Chips",
        [3750] = "100 Chips",
        [3751] = "1,000 Chips",
        [3752] = "10 Wooden Tags",
        [3753] = "100 Wooden Tags",
        [3754] = "1,000 Wooden Tags",
        [3808] = "Toughness Light",
        [3811] = "BOSS Rainbow Mountain",
        [3812] = "Lyemon Tokucha Green Tea",
        [3813] = "C.C. Lemon",
        [3815] = "Cup Sake",
        [3816] = "Bottle of Sake",
        [3817] = "The Premium Malt's Bottle",
        [3818] = "Kakubin",
        [3819] = "Test Cappuccino",
        [3820] = "Test Espresso",
        [3821] = "Test Earl Grey Tea",
        [3822] = "Test Iced Lemon Tea",
        [3823] = "Test Orange Juice",
        [3824] = "Test Carbonara",
        [3825] = "Test Omurice",
        [3826] = "Test Healthy Salad",
        [3827] = "Test Honey Toast",
        [3828] = "Test Specialty Baguette",
        [3829] = "Test Seasonal Cake",
        [3830] = "Test Baked Cheesecake",
        [3831] = "Test Golden Montblanc",
        [3832] = "Test Vanilla Ice Cream",
        [3833] = "Test Special Hayashi Rice",
        [3874] = "Special Chinese Buns",
        [3876] = "High-end Kimchi",
        [3877] = "Mishmash Lunch Box",
        [3878] = "Packed Lunch Box",
        [3879] = "Chilled Noodles Lunch Box",
        [3880] = "Homestyle Lunch Box",
        [3881] = "Special Lunch Box",
        [3882] = "Luxury Kiwami Lunch Box",
        [3884] = "Poseidon Power",
        [3886] = "Muscle Soda",
        [3887] = "Guardian Water",
        [3888] = "Quickness Lemon",
        [3889] = "Sengoku Coffee",
        [3890] = "Kiwami Drink",
        [3891] = "Burn Ointment",
        [3892] = "Cold Medicine",
        [3893] = "Paralytics Poutice",
        [3894] = "Chinese Detoxers",
        [3897] = "Mental Supplements",
        [3898] = "All-Purpose Remedy",
        [3899] = "Restorative Medicine",
        [3900] = "Comforting Medicine",
        [3901] = "Full Recovery Medicine",
        [3902] = "First Aid Kit",
        [3903] = "Premium First Aid Kit",
        [3904] = "Resorative Bolus",
        [3905] = "Revival Bolus",
        [3906] = "Resurrection Bolus",
        [3907] = "Hardball",
        [3908] = "Lamp Stand",
        [3909] = "Throwing Knife",
        [3910] = "Kunai",
        [3911] = "Modded Model Gun",
        [3912] = "Antique Pistol",
        [3913] = "Hand Grenade",
        [3914] = "Rocket Launcher",
        [3921] = "Alcoholic Gas Ball",
        [3922] = "Sleeping Gas Ball",
        [3923] = "Fear Gas Ball",
        [3924] = "Charm Gas Ball",
        [3925] = "Brainwash Gas Ball",
        [3926] = "Rage Gas Ball",
        [3927] = "Daunt Gas Ball",
        [3929] = "Smoke Bomb",
        [3939] = "Daikon Seeds",
        [3940] = "Tomato Seeds",
        [3941] = "Hot Pepper Seeds",
        [3942] = "Potato Seedlings",
        [3943] = "Onion Seedlings",
        [3944] = "Garlic Seedlings",
        [3945] = "Pansy Seeds",
        [3946] = "Lily Seeds",
        [3947] = "Pine Seedlings",
        [3948] = "Rose Seeds",
        [3949] = "Mysterious Compost",
        [3950] = "Mysterious Bulb",
        [3951] = "Mysterious Seeds",
        [3952] = "Carrot Seedlings",
        [3953] = "Daikon",
        [3954] = "Tomato",
        [3955] = "Hot Pepper",
        [3956] = "Potato",
        [3957] = "Onion",
        [3958] = "Garlic",
        [3959] = "Pansy",
        [3960] = "Lily",
        [3961] = "Pine Tree",
        [3962] = "Rose",
        [3963] = "Mysterious Mushroom",
        [3964] = "Mysterious Leaf",
        [3965] = "Mysterious Fruit",
        [3966] = "Mysterious Carrot",
        [3967] = "Plant Growth Stimulant",
        [3968] = "Plant Nutrient",
        [3969] = "Plant Staminan",
        [3970] = "Plant Staminan Spark",
        [3971] = "Bouquet of Pansies",
        [3972] = "Bouquet of Lilies",
        [3973] = "Bouquet of Roses",
        [3974] = "Bonsai",
        [3975] = "Attack Booster",
        [3976] = "Magic Booster",
        [3977] = "Guard Booster",
        [3978] = "Speed Booster",
        [3979] = "Technique Booster",
        [3980] = "Healing Booster",
        [3981] = "Ultra Booster",
        [3982] = "Between Thrill and Passion",
        [3983] = "Think and Grow Confident",
        [3984] = "The Poor Zoo",
        [3985] = "You, Unleashed",
        [3987] = "Special Theory of Relative Brutality",
        [3989] = "Street Dandy",
        [3990] = "Rise Up!",
        [3991] = "Versatility Manual",
        [3992] = "Bravery Manual",
        [3993] = "Wanderer Manual",
        [3994] = "Justice Manual",
        [3995] = "Assassin Manual",
        [3996] = "Underworld Manual",
        [3997] = "Food Service Manual",
        [3998] = "Gallantry Manual",
        [3999] = "Bureaucracy Manual",
        [4000] = "Intimidation Manual",
        [4001] = "Smooth Talker Manual",
        [4002] = "Demo Manual",
        [4003] = "Guitar Manual",
        [4004] = "Tactical Manual",
        [4005] = "Dancing Manual",
        [4006] = "Divination Manual",
        [4007] = "Smile Manual",
        [4008] = "Gambler Manual",
        [4009] = "Sweet Talker Manual",
        [4010] = "Darkness Manual",
        [4021] = "Paper Plate",
        [4022] = "Italian Ring",
        [4023] = "Swiss Watch",
        [4024] = "Worn-out TV",
        [4026] = "Worn-out Microwave",
        [4049] = "Moth",
        [4050] = "Butterfly",
        [4052] = "Rhinoceros Beetle",
        [4053] = "Mantis",
        [4054] = "Spider",
        [4055] = "Scorpion",
        [4056] = "Empty Cicada Shell",
        [4057] = "Dragonfly",
        [4058] = "Stag Beetle",
        [4059] = "Beehive",
        [4063] = "Silver Moth",
        [4064] = "Silver Butterfly",
        [4066] = "Silver Rhinoceros Beetle",
        [4067] = "Silver Mantis",
        [4068] = "Silver Spider",
        [4069] = "Silver Scorpion",
        [4071] = "Silver Dragonfly",
        [4072] = "Silver Stag Beetle",
        [4075] = "Golden Moth",
        [4076] = "Golden Butterfly",
        [4078] = "Golden Rhinoceros Beetle",
        [4079] = "Golden Mantis",
        [4080] = "Golden Spider",
        [4081] = "Golden Scorpion",
        [4083] = "Golden Dragonfly",
        [4084] = "Golden Stag Beetle",
        [4115] = "AiAi",
        [4116] = "GonGon",
        [4117] = "MeeMee",
        [4118] = "Baby",
        [4119] = "Bun-chan the Java Sparrow (White)",
        [4120] = "Bun-chan the Java Sparrow (Pink)",
        [4121] = "Jumbo Bun-chan",
        [4122] = "Kitty Kat (Red)",
        [4123] = "Kitty Kat (Tiger)",
        [4124] = "Kitty Kat (Blue)",
        [4125] = "Kitty Kat (American Short Hair)",
        [4126] = "Kitty Kat (Calico)",
        [4127] = "Robo Chief",
        [4128] = "Robo Manager",
        [4129] = "Pillow Pups (Golden Retriever)",
        [4130] = "Pillow Pups (French Bulldog)",
        [4163] = "Blended Coffee",
        [4164] = "Blue Mountain",
        [4165] = "Mocha",
        [4166] = "Earl Grey Tea",
        [4167] = "Straberry Parfait",
        [4168] = "Chocolate Parfait",
        [4169] = "Special Shortcake",
        [4170] = "Sandwich Set",
        [4171] = "Original Beef Curry",
        [4172] = "Napolitan",
        [4173] = "Smile Burger",
        [4174] = "Teriyaki Smile Burger",
        [4175] = "King Smile Burger",
        [4176] = "Tuna Burger",
        [4177] = "Braised Pork Burger",
        [4178] = "Smile Fries",
        [4179] = "Smile Shake",
        [4180] = "Smile Salad",
        [4181] = "Wild Burger",
        [4182] = "Wild Avocado Burger",
        [4183] = "Wild Chicken Sandwich",
        [4184] = "Wild Fried Chicken",
        [4186] = "Oolong Tea",
        [4187] = "Wette Burger",
        [4188] = "Wette Egg Burger",
        [4189] = "Tomato Onion Soup",
        [4190] = "Wette House Coffee",
        [4191] = "Iced Lemon Tea",
        [4192] = "Kyushu Tonkotsu Ramen",
        [4193] = "Stewed Pork Tonkotsu Ramen",
        [4194] = "Chasu Tonkotsu Ramen",
        [4195] = "Chashu Rice Bowl",
        [4196] = "Fried Rice",
        [4197] = "Gyoza",
        [4198] = "Salted Beef Tongue",
        [4199] = "Grade-A Salted Beef Tongue",
        [4200] = "Kalbi",
        [4201] = "Grade-A Kalbi",
        [4202] = "Sirloin",
        [4203] = "Grade-A Sirloin",
        [4204] = "Harami",
        [4205] = "Grade-A Harami",
        [4206] = "Tripe BBQ",
        [4207] = "Seafood Platter",
        [4208] = "Kimchi Combo",
        [4209] = "Stone-cooked Bibimbap",
        [4210] = "Spicy Beef Soup",
        [4211] = "Grilled Garlic",
        [4212] = "Nishiki Set",
        [4213] = "Miyabi Set",
        [4214] = "Kiwami Set",
        [4215] = "Flower Chirashi",
        [4216] = "Kiwami Chirashi",
        [4217] = "Tuna Rice Bowl",
        [4218] = "Kiwami Seafood Rice Bowl",
        [4219] = "Osuimono",
        [4220] = "Lobster Miso Soup",
        [4221] = "Beef Bowl (Standard)",
        [4222] = "Beef Bowl (Large)",
        [4223] = "Beef Bowl (Extra Large)",
        [4224] = "Beer",
        [4225] = "Miso Soup",
        [4226] = "Absolutey Tasty Takoyaki",
        [4227] = "Cheese and Spicy Fish Roe",
        [4228] = "Welsh Onion Takoyaki",
        [4229] = "Teriyaki and Egg",
        [4231] = "Saucy Yakisoba",
        [4232] = "Spicy Deep-fried Octopus",
        [4234] = "Cucumber Pot",
        [4235] = "The Premium Malt's Draft",
        [4236] = "The Kaku Highball",
        [4237] = "Blood Orange Highball",
        [4238] = "Jim Beam Highball",
        [4239] = "Fresh Cassis and Orange",
        [4240] = "Hot Soba",
        [4241] = "Chilled Soba",
        [4242] = "Chilled Tanuki Soba",
        [4243] = "Chilled Kitsune Soba",
        [4244] = "Egg & Tempura Soba",
        [4245] = "Special Fuji Soba",
        [4246] = "Katsudon",
        [4247] = "Curry & Rice",
        [4248] = "Small Sashimi Platter (3 pcs)",
        [4249] = "Handpicked Vinegar Mackerel",
        [4250] = "Kotchori Salad",
        [4251] = "Edamame",
        [4252] = "Skewer Platter",
        [4253] = "Smelt Fish with Roe",
        [4255] = "Stir-fried Bean Sprouts",
        [4256] = "Draft Beer (Medium)",
        [4257] = "Oolong Tea",
        [4258] = "Kyoho Grape Sour",
        [4259] = "Fresh Grapefruit Sour (Medium)",
        [4260] = "Bahuhai (Whisky & Beer)",
        [4261] = "Gyokuro Green Tea Shochu",
        [4262] = "Kaku Highball",
        [4263] = "Cassis Oolong",
        [4264] = "Nagasaki Champon",
        [4265] = "Vegetable Champon",
        [4266] = "Spicy Champon",
        [4267] = "Nagasaki Saraudon",
        [4268] = "Vetetable Saraudon",
        [4269] = "Thick Saraudon",
        [4270] = "Light Saraudon",
        [4271] = "Spicy Mazemen",
        [4272] = "Extra Beef Mazemen",
        [4273] = "Veggie-filled Soup",
        [4274] = "Half-order Fried Rice",
        [4275] = "Gyoza (5 pcs)",
        [4276] = "Special Tuna Bowl",
        [4277] = "Seafood Chirashi Bowl",
        [4278] = "Special Sushi Zanmai",
        [4279] = "Tuna Zanmai",
        [4280] = "Kokoroiki",
        [4281] = "Tenderloin Steak 200g",
        [4282] = "Rib-eye Steak 300g",
        [4283] = "Sirloin Steak 200g",
        [4284] = "Wild Steak 300g",
        [4285] = "Wild Hamburg Steak 300g",
        [4286] = "Salad",
        [4287] = "Consomme",
        [4288] = "Yamazaki 25 Years Old",
        [4289] = "Yamazaki 18 Years Old",
        [4290] = "Yamazaki 12 Years Old",
        [4291] = "Hakushu 18 Years Old",
        [4293] = "Hakushu",
        [4294] = "Hibiki 30 Years Old",
        [4296] = "Hibiki 21 Years Old",
        [4297] = "Suntory Old Whisky",
        [4298] = "Suntory Brandy V.S.O.P.",
        [4299] = "The Premium Malt's",
        [4300] = "The Macallan 30 Years Old",
        [4301] = "The Macallan 25 Years Old",
        [4302] = "The Macallan  12 Years Old",
        [4303] = "Laphroaig 10 Years Old",
        [4304] = "Bowmore 12 Years Old",
        [4305] = "Glenfiddich 12 Years Old",
        [4306] = "Ballantine's 30 Years Old",
        [4307] = "Ballantine's 17 Years Old",
        [4308] = "Ballantine's 12 Years Old",
        [4309] = "X.O. Deluxe",
        [4310] = "V.O.",
        [4311] = "Courvaisier X.O.",
        [4312] = "The Malt's",
        [4313] = "Carlsberg",
        [4314] = "Kyogetsu Green",
        [4315] = "Beefeater",
        [4316] = "Original Fried Gyoza",
        [4317] = "Tenshinhan",
        [4318] = "Mixed Fried Rice",
        [4319] = "Chinese Soba",
        [4320] = "Sichuan Dandan Noodles",
        [4321] = "Chinese Yakisoba",
        [4322] = "Shrimp in Chili Sauce",
        [4323] = "Pork Pepper Steak",
        [4324] = "Stir-fried Liver and Onion",
        [4325] = "Sweet and Sour Pork",
        [4326] = "Twice-cooked Pork",
        [4327] = "Mapo Tofu",
        [4328] = "Banbanji Chicken Salad",
        [4329] = "Sesame Dumplings",
        [4330] = "Ramen",
        [4331] = "Chasu-men",
        [4332] = "Tecchiri",
        [4333] = "Opulent Tecchiri",
        [4334] = "Tessa",
        [4335] = "Fugu Tempura",
        [4336] = "Deep-fried Fugu",
        [4337] = "The Fuku Fugu Kaiseki",
        [4338] = "The Kotobuki Fugu Kaiseki",
        [4339] = "Benten's Tecchiri Set",
        [4340] = "Daikoku's Tecchiri Set",
        [4341] = "Ebisu's Natural Tecchiri Set",
        [4342] = "Hotei's Natural Tecchiri Set",
        [4343] = "Sushi Balzen",
        [4344] = "Sushu Chikuzen",
        [4345] = "Nigirizen",
        [4346] = "Trop-grade Nigirizen",
        [4347] = "Relaxing Bento",
        [4348] = "Kamizushi Bento",
        [4349] = "Mini Kaiseki Flower",
        [4350] = "Sushi Kaiseki Chotose Course Moon",
        [4351] = "Kaiseki Nishiki Course",
        [4352] = "Gyu-Kaku Beef Ribs",
        [4353] = "Japanese Beef Ribs",
        [4354] = "Salted Tongue",
        [4355] = "Luxury Marbled Salted Tongue",
        [4356] = "Handpicked Beef Course",
        [4357] = "Harami King",
        [4360] = "Cheese Fondue de Chicken Basil",
        [4361] = "Plum Shiso Chilled Noodles",
        [4362] = "Kalbi Rice",
        [4363] = "Gyu-Kabu Ice Cream",
        [4364] = "Crab Nabe",
        [4365] = "Crab Shabu Shabu",
        [4366] = "Crab Amiyaki",
        [4367] = "Crab Nigiri",
        [4368] = "Deep-fried Crab",
        [4369] = "Crab Sushi Platter",
        [4371] = "Crab Nabe Course: Yunagi",
        [4372] = "Crab Nabe Course: Shiosai",
        [4373] = "Crab Kaiseki: Kobai",
        [4374] = "Crab Kaiseki: Watake",
        [4375] = "Crab Kaiseki: Shoro",
        [4376] = "Crab Kaiseki: Fugetsu",
        [4377] = "Takoyaki with Large Octopus",
        [4378] = "Renowned Akashiyaki",
        [4379] = "Green Onion Akashiyaki",
        [4380] = "Shockin' Takoyaki",
        [4381] = "Master's Takoyaki",
        [4382] = "Soup du Jour",
        [4383] = "Sashimi Platter",
        [4384] = "Kyoto Sukiyaki",
        [4385] = "Mattari Kaiseki",
        [4386] = "Hokkori Kaiseki",
        [4387] = "Hannari Kaiseki",
        [4388] = "Snow Crab Course",
        [4389] = "Kyoto Vegetable Tempura",
        [4390] = "Grilled Marbled Wagyu",
        [4391] = "Original Kushikatsu",
        [4392] = "Quail",
        [4393] = "Asparagus",
        [4394] = "Onion",
        [4395] = "Octopus",
        [4396] = "Whiting",
        [4397] = "Lotus Root",
        [4398] = "Garlic Chicken",
        [4399] = "Tsukune",
        [4400] = "Smelt Fish with Roe",
        [4401] = "Cheese",
        [4402] = "Shrimp",
        [4403] = "Scallop",
        [4404] = "Sotenbori Set",
        [4405] = "Hoganji Set",
        [4407] = "Potato Wedges",
        [4408] = "Pasta Sticks",
        [4409] = "Chikuwa Isobeage",
        [4410] = "Yakitori Platter",
        [4412] = "Spicy Cucumber Tataki",
        [4415] = "Matsutake Rice",
        [4416] = "Shokado Bento",
        [4417] = "Bonito Tataki",
        [4418] = "Sweets du Jour",
        [4419] = "Seasonal Stewed Dish",
        [4420] = "Yellowtail Daikon",
        [4421] = "Course du Jour",
        [4422] = "Sashimi Platter",
        [4423] = "Blended Coffee",
        [4424] = "Blue Mountain",
        [4425] = "Cream Latte",
        [4426] = "Strawberry Parfait",
        [4430] = "Bulgogi",
        [4431] = "Samgyeop-sal",
        [4433] = "Samgye-tang",
        [4434] = "Dak-galbi",
        [4436] = "Oi Kimchi",
        [4437] = "Kkakdugi",
        [4439] = "Perilla Leaves",
        [4440] = "Sangchu",
        [4443] = "Chinese Sticky Rice",
        [4444] = "Stir-fried Greens",
        [4445] = "Fried Rice",
        [4446] = "Half-order Fried Rice",
        [4447] = "Mixed Fried Rice",
        [4448] = "Mixed Noodles",
        [4451] = "Shumai",
        [4452] = "Har Gow",
        [4454] = "Almond Jelly",
        [4455] = "Mango Pudding",
        [4456] = "Handmade Meat Bun",
        [4457] = "Bean Paste Bun",
        [4458] = "Shark Fin Bun",
        [4459] = "Shrimp in Chili Sauce",
        [4460] = "Shark Fin Soup",
        [4462] = "Shaoxing Wine",
        [4463] = "Tournedos Rossini",
        [4464] = "Grilled Lobster",
        [4465] = "Roasted Duck",
        [4466] = "Homemade Baguette",
        [4467] = "Red Wine",
        [4468] = "White Wine",
        [4469] = "Dessert du Jour",
        [4470] = "Lamb Chops with Basil Sauce",
        [4471] = "Shrimp and Urchin Appetizer (with Caviar)",
        [4472] = "Seafood Tartare",
        [4473] = "Addicting Bite-size Karaage",
        [4474] = "French Fries",
        [4475] = "Special Frankfurter",
        [4476] = "Super Spicy Frankfurter",
        [4477] = "Fluffy Croquettes",
        [4478] = "Suntory Mineral Water",
        [4479] = "Suntory Black Oolong Tea",
        [4480] = "GREEN DAKARA",
        [4481] = "Apple Defense",
        [4482] = "Peach Step",
        [4483] = "Astringent Gauze",
        [4484] = "Incendiary Grenade",
        [4485] = "Liquid Nitrogen Spray",
        [4486] = "Lightning Bomb",
        [4487] = "Physical Booster",
        [4488] = "Mental Booster",
        [4659] = "Matured Roast Steak",
        [4962] = "Roasted Chestnuts",
        [4963] = "Grandma's Cookies",
        [4964] = "High Payout Token",
        [4965] = "Hades Token: God",
        [4966] = "Hades Token: Dark Lord",
        [4967] = "Hades Token: Purple 7",
        [4968] = "Return Token: God",
        [4969] = "Return Token: Red 7",
        [4970] = "Souten Token: Deathmatch",
        [4971] = "Souten Token: Passing Heavens Ritual",
        [4972] = "Beast King Token: Savanna Chance",
        [4973] = "Wood Block",
        [4974] = "Sturdy Lumber",
        [4975] = "Quality Lumber",
        [4976] = "High-quality Lumber",
        [4977] = "Highest-quality Lumber",
        [4978] = "Ragged Cloth",
        [4979] = "Thick Cloth",
        [4980] = "Beautiful Cloth",
        [4981] = "High-quality Cloth",
        [4982] = "Dirty Iron",
        [4983] = "Sturdy Iron",
        [4984] = "Refined Iron",
        [4985] = "Reinforced Alloy",
        [4986] = "Tempered Steel",
        [4987] = "Superalloy Steel",
        [4988] = "Cloudy Pane",
        [4989] = "Clear Pane",
        [4990] = "Immaculate Pane",
        [4991] = "Tempered Glass",
        [4992] = "Tempered Safety Glass",
        [4993] = "Cheap Plastic",
        [4994] = "Sturdy Plastic",
        [4995] = "Dull Aluminum",
        [4996] = "Gleaming Aluminum",
        [4997] = "Rough Hide",
        [4998] = "Thick Hide",
        [4999] = "Smooth Hide",
        [5000] = "High-end Leather",
        [5001] = "Full-grain Leather",
        [5002] = "Odd Stone",
        [5003] = "Mysterious Stone",
        [5004] = "Impure Lead",
        [5005] = "High-purity Lead",
        [5006] = "High-density Metal",
        [5007] = "Tungsten",
        [5008] = "Nail",
        [5009] = "Metal Wire",
        [5010] = "Pearl",
        [5011] = "Silver Ingot",
        [5012] = "Gold Ingot",
        [5013] = "Raw Platinum Ore",
        [5014] = "Raw Ruby Ore",
        [5015] = "Raw Sapphire Ore",
        [5016] = "Raw Diamond Ore",
        [5033] = "Handmade Meat Bun",
        [5034] = "Bean Paste Bun",
        [5035] = "Shark Fin Bun",
        [5036] = "Chinese Sticky Rice",
        [5038] = "Chili Bean Paste",
        [5042] = "Fresh Milk",
        [5043] = "Hot Milk",
        [5044] = "Soft Serve Ice Cream",
        [5048] = "Chilled Tomatoes",
        [5049] = "Caesar Salad",
        [5050] = "Scone",
        [5051] = "Pain d'épi",
        [5052] = "Special Pancakes",
        [5053] = "Eomeoni's Kimchi",
        [5054] = "Kanitama",
        [5055] = "Mapo Tofu",
        [5056] = "Sweet and Sour Pork",
        [5057] = "Pork Pepper Steak",
        [5058] = "Water",
        [5059] = "Cooking Manual",
        [5060] = "Dominance Manual",
        [5061] = "Turmeric Tablets",
        [5067] = "Shrimp Karaage",
        [5068] = "Crunchy-fried Chicken Skin",
        [5070] = "Development Kit (Beginner)",
        [5071] = "Development Kit (Intermediate)",
        [5072] = "Development Kit (Advanced)",
        [5074] = "Chocolate Parfait",
        [5075] = "Napolitan",
        [5076] = "Chashu Tonkotsu Ramen",
        [5077] = "Chashu Rice Bowl",
        [5078] = "Pasta Sticks",
        [5079] = "Tournedos Rossini",
        [5080] = "Crunchy-fried Chicken Skin",
        [5081] = "Fruit Platter",
        [5082] = "Strawberry Parfait",
        [5083] = "Beer",
        [5084] = "Red Wine",
        [5085] = "Water",
        [5086] = "Kanitama",
        [5087] = "Mapo Tofu",
        [5088] = "Chinese Sticky Rice",
        [5089] = "Mixed Fried Rice",
        [5090] = "Mixed Noodles",
        [5091] = "Sweet and Sour Pork",
        [5092] = "Har Gow",
        [5093] = "Almond Jelly",
        [5094] = "Oolong Tea",
        [5095] = "Shaoxing Wine",
        [5168] = "Pickles",
        [5169] = "Rice",
        [5173] = "Special Job: Devil Rocker",
        [5174] = "Special Job: Matriarch",
        [5175] = "Iconic Characters Costume Set",
        [5176] = "Talented Staff Pack 1 (Legends)",
        [5177] = "Talented Staff Pack 2 (Reliable Allies)",
        [5178] = "Talented Staff Pack 3 (Hostess Legends)",
        [5179] = "Materials Pack - Matsu",
        [5180] = "Materials Pack - Take",
        [5181] = "Materials Pack - Ume",
        [5182] = "Costume Head (All) & Gear Set",
        [5190] = "Personality Enhancement Pack",
        [5191] = "Main Job Assistance Pack",
        [5192] = "Majima Construction Set",
        [5194] = "Job Change Assistance Pack 1",
        [5195] = "Job Change Assistance Pack 2",
        [5196] = "Battle Assistance Pack",
        [5197] = "Pachislot Machines 4 Pack",
        [5198] = "Upstart Assistance Pack 1",
        [5199] = "Upstart Assistance Pack 2",
        [5200] = "Upstart Assistance Pack 3",
        [5201] = "Upstart Assistance Pack 4",
        [5202] = "Upstart Assistance Pack 5",
        [5203] = "Upstart Assistance Pack 6",
        [5204] = "Upstart Assistance Pack 7",
        [5206] = "Edamame",
        [5208] = "Complimentary Rice",
        [5209] = "Yamazaki Highball",
        [5210] = "Citrus Lemon Highball",
        [5211] = "Oolong Tea",
        [5212] = "Yuzu Chicken & Spinach Soba",
        [5213] = "Sashimi Platter (5 pcs)",
        [5214] = "Salted Yakisoba (with Sichuan Chili Oil)",
        [5215] = "Japanese Black Mince Cutlet",
        [5216] = "Ice Brûlée",
        [5217] = "Lemon Sour",
        [5218] = "The Premium Malt's Draft",
        [5219] = "Honey Pepper Dwaeji Kalbi",
        [5220] = "Crab Nabe Course: Hiyori",
        [5221] = "Fried Pasta",
        [5222] = "Mixed Nuts",
        [5223] = "Tomato & Mozzarella Caprese",
        [5224] = "Pickled Vegetable Sticks",
        [5225] = "Hot Nachos in Meat Sauce",
        [5226] = "Prosciutto Platter",
        [5227] = "Caesar Salad",
        [5228] = "Chicken Basket",
        [5229] = "Onion Rings",
        [5230] = "Bee Garlic Chicken Rice",
        [5231] = "Oolong Tea",
        [5232] = "Beer",
        [5233] = "Red Wine",
        [5234] = "White Wine",
        [5235] = "Crème Brûlée Ice Cake",
        [5236] = "Ichiban Senbei (Shoyu)",
        [5237] = "Ichiban Senbei (Salt)",
        [5238] = "Ichiban Fried Senbei",
        [5239] = "Nozaki's Corned Beef",
        [5240] = "Pine Candy",
        [5241] = "Empty Cough Drop Tin",
        [5242] = "Kao Attack ZERO",
        [5243] = "Cemedine Super X",
        [5244] = "SEGA Taiyaki",
        [5245] = "Oragamachi Natto",
        [5246] = "Scalp D",
        [5247] = "Twist Fragrance",
        [5248] = "Miracle Kimchi",
        [5249] = "SEGA Taiyaki B (Temp)",
        [5605] = "New Karaoke (Soundtrack & CD Set)",
        [5606] = "Kiryu's Greatest Hits (Full & CD Set)",
        [5607] = "Majima's Great Hits (Full & CD Set)",
        [5608] = "Karaoke Classics (Full & CD Set)",
        [5609] = "Legendary First Aid Kit",
        [5613] = "Almond Jelly",
        [5614] = "Mango Pudding",
        [5628] = "Gold Champagne",
        [5629] = "Rosé Champagne",
        [5630] = "Black Champagne",
        [5631] = "Egg",
        [5632] = "Quality Egg",
        [5633] = "Golden Egg",
        [5635] = "10 yen",
        [5636] = "100 yen",
        [5637] = "500 yen",
        [5639] = "Special Costume: Kazuma Kiryu Yakuza 0 Ver. (Kasuga)",
        [5640] = "Special Costume: Goro Majima Yakuza 0 Ver. (Kasuga)",
        [5641] = "Special Costume: Ryuji Goda (Kasuga)",
        [5642] = "Special Costume: Daigo Dojima (Kasuga)",
        [5643] = "Special Costume: Kaoru Sayama (Saeko)",
        [5644] = "Special Costume: Haruka Sawamura (Eri)",
        [5645] = "Special Costume: Taiga Saejima (Kasuga)",
        [5646] = "1,000 yen",
        [5647] = "5,000 yen",
        [5648] = "10,000 yen",
        [5649] = "100,000 yen",
        [5650] = "Special Costume: Makoto Date (Kasuga)",
        [5651] = "Low Sodium Champon",
        [5654] = "Ultimate Pork Bun",
        [5655] = "Imuraya Steamed Bun",
        [5656] = "Imuraya Yokan",
        [5657] = "Imuraya Castella",
        [5692] = "Yakuza: Like A Dragon Expansion Kit",
        [5693] = "Bleach Japan Outfit Set",
        [5694] = "Double Heroine Outfit Set",
        [5695] = "Premium Item Set",
        [5696] = "Premium Outfit Set",
        [5697] = "Level Up Pack",
        [5698] = "Level Booster",
        [5699] = "Crafting Materials Bundle",
        [5700] = "Stat Boost Bundle",
    };

    public MainForm()
    {
        SuspendLayout();
        Text = "Yakuza LAD Save Editor";
        Size = new System.Drawing.Size(1100, 750);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new System.Drawing.Size(900, 600);

        try { Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath)!; } catch { }

        var toolbar = new PanelControl { Dock = DockStyle.Top, Height = 40 };
        var btnOpen = new SimpleButton { Text = "Open", Dock = DockStyle.Left, Width = 90, Padding = new Padding(4) };
        var btnSave = new SimpleButton { Text = "Save", Dock = DockStyle.Left, Width = 90 };
        var btnSaveAs = new SimpleButton { Text = "Save As", Dock = DockStyle.Left, Width = 90 };
        btnOpen.Click += (_, _) => DoOpen();
        btnSave.Click += (_, _) => DoSave(false);
        btnSaveAs.Click += (_, _) => DoSave(true);
        toolbar.Controls.AddRange([btnSaveAs, btnSave, btnOpen]);

        var footer = new PanelControl { Dock = DockStyle.Bottom, Height = 28 };
        footer.Controls.Add(new LabelControl
        {
            Text = "By zXIJz", Dock = DockStyle.Left, Padding = new Padding(8, 0, 0, 0),
            Appearance = { Font = new Font("Segoe UI", 11f, FontStyle.Regular) }
        });

        _welcome = new LabelControl
        {
            Text = "Open a decrypted Yakuza: Like a Dragon save file to begin editing.",
            Dock = DockStyle.Fill, AutoSizeMode = LabelAutoSizeMode.None,
            Appearance = { Font = new Font("Segoe UI", 14f),
                TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center,
                                VAlignment = DevExpress.Utils.VertAlignment.Center } }
        };

        _tabs = new XtraTabControl { Dock = DockStyle.Fill, Visible = false };
        _tabs.TabPages.AddRange([
            BuildGeneralTab(), BuildCharactersTab(), BuildJobsTab(), BuildInventoryTab(),
            BuildPointsTab(), BuildManagementTab(), BuildSceneTab(),
            BuildKaraokeTab(), BuildDragonKartTab(),
            BuildMinigameTab("Batting", "mg_batting"), BuildMinigameTab("Golf", "mg_golf"),
            BuildMinigameTab("Pinball", "mg_pinball"), BuildMinigameTab("Mahjong", "mg_mahjong"),
            BuildMinigameTab("Can Collection", "mg_can_collection"), BuildMinigameTab("Cinema", "mg_cinema"),
        ]);

        Controls.Add(_welcome);
        Controls.Add(_tabs);
        Controls.Add(footer);
        Controls.Add(toolbar);
        ResumeLayout(false);
    }

    SpinEdit MakeSpin(Control parent, string label, int x, int y, long max = 999_999_999_999, int w = 220)
    {
        parent.Controls.Add(new LabelControl { Text = label, Location = new Point(x, y) });
        var s = new SpinEdit { Location = new Point(x, y + 20), Width = w };
        s.Properties.MaxValue = max;
        s.Properties.MinValue = 0;
        s.Properties.IsFloatValue = false;
        parent.Controls.Add(s);
        return s;
    }

    GridControl MakeGrid(XtraTabPage page)
    {
        var gc = new GridControl { Dock = DockStyle.Fill };
        var gv = new GridView(gc);
        gc.MainView = gv;
        gv.OptionsView.ShowGroupPanel = false;
        gv.OptionsBehavior.Editable = true;
        gv.OptionsFilter.AllowFilterEditor = true;
        gv.OptionsView.ColumnAutoWidth = true;
        gv.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Fast;
        page.Controls.Add(gc);
        return gc;
    }

    void SetupGrid(GridControl gc, bool hideUnderscoreCols = true)
    {
        var gv = (GridView)gc.MainView;
        if (hideUnderscoreCols)
            foreach (GridColumn col in gv.Columns)
                if (col.FieldName.StartsWith("_")) col.Visible = false;
        gv.BestFitColumns();
    }

    XtraTabPage BuildGeneralTab()
    {
        var page = new XtraTabPage { Text = "General" };
        var sc = new XtraScrollableControl { Dock = DockStyle.Fill, Padding = new Padding(20) };
        _titleLabel = new LabelControl { Text = "", Location = new Point(20, 16), AutoSizeMode = LabelAutoSizeMode.Horizontal,
            Appearance = { Font = new Font("Segoe UI", 12f, FontStyle.Bold) } };
        sc.Controls.Add(_titleLabel);

        sc.Controls.Add(new LabelControl { Text = "Save Title", Location = new Point(20, 50) });
        _saveTitle = new TextEdit { Location = new Point(20, 70), Width = 400 };
        sc.Controls.Add(_saveTitle);

        sc.Controls.Add(new LabelControl { Text = "Save Subtitle", Location = new Point(20, 100) });
        _saveSubtitle = new TextEdit { Location = new Point(20, 120), Width = 400 };
        sc.Controls.Add(_saveSubtitle);

        _money = MakeSpin(sc, "Money", 20, 160);
        _mgmtFunds = MakeSpin(sc, "Management Funds", 20, 220);
        
        _playTime = MakeSpin(sc, "Play Time (Seconds)", 20, 280, long.MaxValue);
        
        _difficulty = MakeSpin(sc, "Difficulty", 20, 340, 10);
        _stomachNow = MakeSpin(sc, "Stomach Now", 300, 160, 9999);
        _stomachMax = MakeSpin(sc, "Stomach Max", 300, 220, 9999);
        _clothing = MakeSpin(sc, "Clothing ID", 300, 280, 99999);
        _drunkMax = MakeSpin(sc, "Drunk Max Level", 300, 340, 99);
        _battleStyle = MakeSpin(sc, "Battle Style ID", 580, 220, 99999);

        _iconPanel = new PanelControl { Location = new Point(580, 16), Size = new System.Drawing.Size(260, 180), Visible = false };
        _iconPanel.Controls.Add(new LabelControl { Text = "Save Icon (icon0.png)", Location = new Point(6, 4),
            Appearance = { Font = new Font("Segoe UI", 9f, FontStyle.Bold) } });
        _iconPreview = new PictureEdit { Location = new Point(6, 24), Size = new System.Drawing.Size(228, 128),
            Properties = { SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom, ReadOnly = true } };
        _iconPanel.Controls.Add(_iconPreview);
        _iconChangeBtn = new SimpleButton { Text = "Change Icon", Location = new Point(6, 155), Width = 120 };
        _iconChangeBtn.Click += (_, _) => DoChangeIcon();
        _iconPanel.Controls.Add(_iconChangeBtn);
        sc.Controls.Add(_iconPanel);

        page.Controls.Add(sc);
        return page;
    }

    void DoChangeIcon()
    {
        using var dlg = new OpenFileDialog { Filter = "All Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.tif;*.webp;*.ico|PNG|*.png|JPEG|*.jpg;*.jpeg|BMP|*.bmp|All Files|*.*", Title = "Select Save Icon" };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        try
        {
            var bytes = File.ReadAllBytes(dlg.FileName);
            using var ms = new MemoryStream(bytes);
            var img = Image.FromStream(ms);

            if (img.Width != 228 || img.Height != 128)
            {
                var resized = new Bitmap(228, 128);
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, 0, 0, 228, 128);
                }
                img.Dispose();
                img = resized;
            }

            _iconPreview.Image = img;
            _newIconPath = dlg.FileName;
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Failed to load image:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    void SaveIcon()
    {
        if (_newIconPath == null || _save == null) return;
        var sceDir = _save.GetSceSysDir();
        if (sceDir == null) return;

        var destPath = Path.Combine(sceDir, "icon0.png");
        var img = _iconPreview.Image;
        if (img == null) return;

        using var ms = new MemoryStream();
        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        File.WriteAllBytes(destPath, ms.ToArray());
        _newIconPath = null;
    }

    XtraTabPage BuildCharactersTab()
    {
        var page = new XtraTabPage { Text = "Characters" };
        _charGrid = MakeGrid(page);
        return page;
    }

    XtraTabPage BuildJobsTab()
    {
        var page = new XtraTabPage { Text = "Jobs" };
        _jobGrid = MakeGrid(page);
        return page;
    }

    XtraTabPage BuildInventoryTab()
    {
        var page = new XtraTabPage { Text = "Inventory" };
        var split = new SplitContainerControl { Dock = DockStyle.Fill, Horizontal = false, SplitterPosition = 300 };
        page.Controls.Add(split);

        split.Panel1.Controls.Add(new LabelControl { Text = "Carry Items", Dock = DockStyle.Top,
            Appearance = { Font = new Font("Segoe UI", 10f, FontStyle.Bold) }, Padding = new Padding(4) });
        _invGrid = new GridControl { Dock = DockStyle.Fill };
        var gv1 = new GridView(_invGrid); _invGrid.MainView = gv1;
        gv1.OptionsView.ShowGroupPanel = false; gv1.OptionsBehavior.Editable = true;
        gv1.OptionsView.ColumnAutoWidth = true;
        split.Panel1.Controls.Add(_invGrid);

        split.Panel2.Controls.Add(new LabelControl { Text = "Item Box", Dock = DockStyle.Top,
            Appearance = { Font = new Font("Segoe UI", 10f, FontStyle.Bold) }, Padding = new Padding(4) });
        _boxGrid = new GridControl { Dock = DockStyle.Fill };
        var gv2 = new GridView(_boxGrid); _boxGrid.MainView = gv2;
        gv2.OptionsView.ShowGroupPanel = false; gv2.OptionsBehavior.Editable = true;
        gv2.OptionsView.ColumnAutoWidth = true;
        split.Panel2.Controls.Add(_boxGrid);

        return page;
    }

    XtraTabPage BuildPointsTab()
    {
        var page = new XtraTabPage { Text = "Points" };
        _ptsGrid = MakeGrid(page);
        return page;
    }

    XtraTabPage BuildManagementTab()
    {
        var page = new XtraTabPage { Text = "Management" };
        var sc = new XtraScrollableControl { Dock = DockStyle.Fill, Padding = new Padding(20) };
        _compRank = MakeSpin(sc, "Max Rank", 20, 16, 999);
        _compTarget = MakeSpin(sc, "Target Rank", 20, 76, 999);
        _compTurns = MakeSpin(sc, "Turn Count", 20, 136, 9999);
        _compPeriod = MakeSpin(sc, "Period", 300, 16, 999);
        _compStock = MakeSpin(sc, "Stock Price Index", 300, 76, 99999);
        _compScale = MakeSpin(sc, "Scale ID", 300, 136, 99999);
        page.Controls.Add(sc);
        return page;
    }

    XtraTabPage BuildSceneTab()
    {
        var page = new XtraTabPage { Text = "Scene" };
        var sc = new XtraScrollableControl { Dock = DockStyle.Fill, Padding = new Padding(20) };
        _sceneId = MakeSpin(sc, "Scene ID", 20, 16, 99999);
        _sceneConfig = MakeSpin(sc, "Scene Config ID", 20, 76, 99999);
        _stage = MakeSpin(sc, "Stage", 300, 16, 99999);
        _dayNight = MakeSpin(sc, "Day/Night", 300, 76, 10);
        page.Controls.Add(sc);
        return page;
    }

    XtraTabPage BuildKaraokeTab()
    {
        var page = new XtraTabPage { Text = "Karaoke" };
        _karaokeGrid = MakeGrid(page);
        return page;
    }

    XtraTabPage BuildDragonKartTab()
    {
        var page = new XtraTabPage { Text = "Dragon Kart" };
        _cartGrid = MakeGrid(page);
        return page;
    }

    XtraTabPage BuildMinigameTab(string name, string section)
    {
        var page = new XtraTabPage { Text = name, Tag = section };
        var gc = MakeGrid(page);
        _mgGrids[section] = gc;
        return page;
    }

    void DoOpen()
    {
        using var dlg = new OpenFileDialog { Filter = "Save Files|*.sav;*.json|All Files|*.*", Title = "Open Decrypted Save" };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        try
        {
            _save = new SaveData();
            _save.Load(dlg.FileName);
            PopulateAll();
            _welcome.Visible = false;
            _tabs.Visible = true;
            _tabs.BringToFront();
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Failed to load save:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    void PopulateAll()
    {
        SuspendLayout();

        var sfoPath = _save.GetParamSfoPath();
        long secondsFromSfo = -1;
        if (sfoPath != null)
        {
            _saveTitle.Text = SaveData.ReadSfoString(sfoPath, "MAINTITLE") ?? _save.GetTitle();
            _saveSubtitle.Text = SaveData.ReadSfoString(sfoPath, "SUBTITLE") ?? _save.GetSubtitle();
            _saveTitle.Enabled = true;
            _saveSubtitle.Enabled = true;

            var detail = SaveData.ReadSfoString(sfoPath, "DETAIL");
            if (detail != null)
            {
                var match = System.Text.RegularExpressions.Regex.Match(detail, @"Play Time:\s*(\d+):(\d+):(\d+)");
                if (match.Success)
                {
                    secondsFromSfo = int.Parse(match.Groups[1].Value) * 3600 +
                                     int.Parse(match.Groups[2].Value) * 60 +
                                     int.Parse(match.Groups[3].Value);
                }
            }
        }
        else
        {
            _saveTitle.Text = _save.GetTitle();
            _saveSubtitle.Text = _save.GetSubtitle();
            _saveTitle.Enabled = false;
            _saveSubtitle.Enabled = false;
        }
        _titleLabel.Text = $"{_saveTitle.Text} — {_saveSubtitle.Text}";

        var iconPath = _save.GetIconPath();
        if (iconPath != null)
        {
            var bytes = File.ReadAllBytes(iconPath);
            using var ms = new MemoryStream(bytes);
            _iconPreview.Image = Image.FromStream(new MemoryStream(bytes));
            _iconPanel.Visible = true;
        }
        else if (_save.GetSceSysDir() != null)
        {
            _iconPreview.Image = null;
            _iconPanel.Visible = true;
        }
        else
        {
            _iconPanel.Visible = false;
        }
        _newIconPath = null;

        _money.EditValue = _save.GetPoint(1);
        _mgmtFunds.EditValue = _save.GetPoint(330);
        _playTime.EditValue = secondsFromSfo >= 0 ? secondsFromSfo : (_save.GetPlayTick() / 1000);
        _difficulty.EditValue = _save.GetDifficulty();
        _stomachNow.EditValue = (decimal)_save.GetStomachNow();
        _stomachMax.EditValue = (decimal)_save.GetStomachMax();
        _clothing.EditValue = _save.GetClothing();
        _drunkMax.EditValue = _save.GetDrunkMaxLevel();
        _battleStyle.EditValue = _save.GetBattleStyle();

        _charGrid.DataSource = _save.GetCharactersTable();
        SetupGrid(_charGrid);
        ((GridView)_charGrid.MainView).Columns["Name"].OptionsColumn.AllowEdit = false;

        _jobGrid.DataSource = _save.GetJobsTable();
        SetupGrid(_jobGrid);

        var invDt = _save.GetInventoryTable();
        AddItemNameColumn(invDt);
        _invGrid.DataSource = invDt;
        SetupGrid(_invGrid);
        ((GridView)_invGrid.MainView).Columns["Item Name"].OptionsColumn.AllowEdit = false;

        var boxDt = _save.GetBoxItemTable();
        AddItemNameColumn(boxDt);
        _boxGrid.DataSource = boxDt;
        SetupGrid(_boxGrid);
        ((GridView)_boxGrid.MainView).Columns["Item Name"].OptionsColumn.AllowEdit = false;

        _ptsGrid.DataSource = _save.GetPointsTable();
        SetupGrid(_ptsGrid);

        _karaokeGrid.DataSource = _save.GetKaraokeTable();
        SetupGrid(_karaokeGrid);

        _cartGrid.DataSource = _save.GetDragonCartTable();
        SetupGrid(_cartGrid);

        _compRank.EditValue = _save.GetCompanyMaxRank();
        _compTarget.EditValue = _save.GetCompanyTargetRank();
        _compTurns.EditValue = _save.GetCompanyTurnCount();
        _compPeriod.EditValue = _save.GetCompanyPeriod();
        _compStock.EditValue = _save.GetCompanyStockIdx();
        _compScale.EditValue = _save.GetCompanyScale();

        _sceneId.EditValue = _save.GetSceneId();
        _sceneConfig.EditValue = _save.GetSceneConfigId();
        _stage.EditValue = _save.GetStage();
        _dayNight.EditValue = _save.GetDayNight();

        foreach (var kv in _mgGrids)
        {
            kv.Value.DataSource = _save.GetMinigameTable(kv.Key);
            SetupGrid(kv.Value);
        }

        ResumeLayout(true);
    }

    static void AddItemNameColumn(DataTable dt)
    {
        dt.Columns.Add("Item Name", typeof(string));
        foreach (DataRow row in dt.Rows)
        {
            int id = (int)row["Item ID"];
            row["Item Name"] = ItemNames.TryGetValue(id, out var n) ? n : "";
        }
    }

    void CollectAll()
    {
        _save.SetTitle(_saveTitle.Text);
        _save.SetSubtitle(_saveSubtitle.Text);
        var sfoPath = _save.GetParamSfoPath();
        if (sfoPath != null)
        {
            SaveData.WriteSfoString(sfoPath, "MAINTITLE", _saveTitle.Text);
            SaveData.WriteSfoString(sfoPath, "SUBTITLE", _saveSubtitle.Text);
            
            var detail = SaveData.ReadSfoString(sfoPath, "DETAIL");
            if (detail != null)
            {
                long playTimeSeconds = Convert.ToInt64(_playTime.EditValue);
                var ts = TimeSpan.FromSeconds(playTimeSeconds);
                string tsStr = $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
                
                detail = System.Text.RegularExpressions.Regex.Replace(detail, @"Play Time:\s*[0-9:]+", $"Play Time: {tsStr}");
                detail = System.Text.RegularExpressions.Regex.Replace(detail, @"Money:\s*\d+", $"Money: {Convert.ToInt64(_money.EditValue)}");
                
                SaveData.WriteSfoString(sfoPath, "DETAIL", detail);
            }
        }
        _save.SetDifficulty(Convert.ToInt32(_difficulty.EditValue));
        _save.SetStomach((float)Convert.ToDouble(_stomachNow.EditValue), (float)Convert.ToDouble(_stomachMax.EditValue));
        _save.SetClothing(Convert.ToInt32(_clothing.EditValue));
        _save.SetDrunkMaxLevel(Convert.ToInt32(_drunkMax.EditValue));
        _save.SetBattleStyle(Convert.ToInt32(_battleStyle.EditValue));

        if (_charGrid.DataSource is DataTable charDt) _save.ApplyCharactersTable(charDt);
        if (_jobGrid.DataSource is DataTable jobDt) _save.ApplyJobsTable(jobDt);
        if (_invGrid.DataSource is DataTable invDt) _save.ApplyInventoryTable(invDt);
        if (_boxGrid.DataSource is DataTable boxDt) _save.ApplyBoxItemTable(boxDt);
        if (_ptsGrid.DataSource is DataTable ptsDt) _save.ApplyPointsTable(ptsDt);
        if (_karaokeGrid.DataSource is DataTable karDt) _save.ApplyKaraokeTable(karDt);
        if (_cartGrid.DataSource is DataTable cartDt) _save.ApplyDragonCartTable(cartDt);

        _save.SetPoint(1, Convert.ToInt64(_money.EditValue));
        _save.SetPoint(330, Convert.ToInt64(_mgmtFunds.EditValue));
        _save.SetPlayTick(Convert.ToInt64(_playTime.EditValue) * 1000);

        _save.SetCompanyMaxRank(Convert.ToInt32(_compRank.EditValue));
        _save.SetCompanyTargetRank(Convert.ToInt32(_compTarget.EditValue));
        _save.SetCompanyTurnCount(Convert.ToInt32(_compTurns.EditValue));
        _save.SetCompanyPeriod(Convert.ToInt32(_compPeriod.EditValue));
        _save.SetCompanyStockIdx(Convert.ToInt32(_compStock.EditValue));
        _save.SetCompanyScale(Convert.ToInt32(_compScale.EditValue));

        _save.SetSceneId(Convert.ToInt32(_sceneId.EditValue));
        _save.SetSceneConfigId(Convert.ToInt32(_sceneConfig.EditValue));
        _save.SetStage(Convert.ToInt32(_stage.EditValue));
        _save.SetDayNight(Convert.ToInt32(_dayNight.EditValue));

        foreach (var kv in _mgGrids)
            if (kv.Value.DataSource is DataTable mgDt) _save.ApplyMinigameTable(kv.Key, mgDt);
    }

    void DoSave(bool saveAs)
    {
        if (_save == null) return;
        CollectAll();
        string path = _save.FilePath;
        if (saveAs)
        {
            using var dlg = new SaveFileDialog { Filter = "Save Files|*.sav|All Files|*.*", FileName = Path.GetFileName(path) };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            path = dlg.FileName;
        }
        try
        {
            _save.Save(path);
            SaveIcon();
            XtraMessageBox.Show("Save successful!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Failed to save:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
