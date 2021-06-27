using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Tactics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    /// <summary>
    /// Base implementation of content storage.
    /// </summary>
    internal sealed class PersonVisualizationContentStorage : IPersonVisualizationContentStorage
    {
        private readonly Dictionary<string, AnimalPart[]> _animalParts;
        private readonly Dictionary<string, BodyPart[]> _bodyParts;
        private readonly Dictionary<string, HandPart[]> _handParts;
        private readonly Dictionary<string, HeadPart[]> _headParts;

        public PersonVisualizationContentStorage()
        {
            _bodyParts = new Dictionary<string, BodyPart[]>();
            _handParts = new Dictionary<string, HandPart[]>();
            _headParts = new Dictionary<string, HeadPart[]>();

            _animalParts = new Dictionary<string, AnimalPart[]>();
        }

        private void LoadAnimalParts(ContentManager content)
        {
            LoadSpecificAnimalParts(content, "hunter");
        }

        private void LoadBodyParts(ContentManager content)
        {
            Texture2D load(string equipmentName, string partName)
            {
                return content.Load<Texture2D>($"Sprites/game-objects/Equipments/BodyParts/{equipmentName}/{partName}");
            }

            _bodyParts.Add("work-clothes", new[]
            {
                new BodyPart(BodyPartType.Chest, load("WorkClothes", "Body")),
                new BodyPart(BodyPartType.LegsIdle, load("WorkClothes", "LegsIdle")),
                new BodyPart(BodyPartType.LegsCombat, load("WorkClothes", "LegsCombat"))
            });

            _bodyParts.Add("traveler-camisole", new[]
            {
                new BodyPart(BodyPartType.Chest, load("TravelerCamisole", "Body")),
                new BodyPart(BodyPartType.LegsIdle, load("TravelerCamisole", "LegsIdle")),
                new BodyPart(BodyPartType.LegsCombat, load("TravelerCamisole", "LegsCombat")),
                new BodyPart(BodyPartType.ArmLeft, load("TravelerCamisole", "ArmLeftSimple")),
                new BodyPart(BodyPartType.ArmLeftTwoHanded, load("TravelerCamisole", "ArmLeftTwoHanded")),
                new BodyPart(BodyPartType.ArmRightSimple, load("TravelerCamisole", "ArmRightSimple")),
                new BodyPart(BodyPartType.ArmRightTwoHanded, load("TravelerCamisole", "ArmRightTwoHanded"))
            });
        }

        private void LoadHandParts(ContentManager content)
        {
            const string PATH_TO_HAND_PARTS = "Sprites/game-objects/Equipments/HandParts/";

            Texture2D load(string name) { return content.Load<Texture2D>(PATH_TO_HAND_PARTS + name); }

            _handParts.Add("short-sword", new[]
            {
                new HandPart(HandPartType.Base, load("ShortSwordBase"))
            });

            _handParts.Add("great-sword", new[]
            {
                new HandPart(HandPartType.Base, load("GreatSwordBase"))
            });

            _handParts.Add("combat-staff", new[]
            {
                new HandPart(HandPartType.Base, load("CombatStaffBase"))
            });

            _handParts.Add("tribal-spear", new[]
            {
                new HandPart(HandPartType.Base, load("TribalSpearBase"))
            });

            _handParts.Add("wooden-shield", new[]
            {
                new HandPart(HandPartType.Base, load("WoodenShieldBase")),
                new HandPart(HandPartType.BaseBack, load("WoodenShieldBase"))
            });
        }

        private void LoadHeadParts(ContentManager content)
        {
            const string PATH_TO_HEAD_PARTS = "Sprites/game-objects/Equipments/HeadParts/";

            _headParts.Add("knitted-hat", new[]
            {
                new HeadPart(HeadPartType.Base, content.Load<Texture2D>(PATH_TO_HEAD_PARTS + "KnittedHatBase"))
            });

            _headParts.Add("steel-helmet", new[]
            {
                new HeadPart(HeadPartType.Base, content.Load<Texture2D>(PATH_TO_HEAD_PARTS + "SteelHelmetBase")),
                new HeadPart(HeadPartType.Inside, content.Load<Texture2D>(PATH_TO_HEAD_PARTS + "SteelHelmetInside"))
            });
        }

        private void LoadHumanOutlinedParts(ContentManager content)
        {
            const string PATH_TO_HUMAN_PARTS = "Sprites/game-objects/Human/Outlined/";

            Texture2D load(string name) { return content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + name); }

            _bodyParts.Add("Human/Outlined", new[]
            {
                new BodyPart(BodyPartType.Head, load("Head")),
                new BodyPart(BodyPartType.Chest, load("Body")),
                new BodyPart(BodyPartType.LegsIdle, load("LegsIdle")),
                new BodyPart(BodyPartType.LegsCombat, load("LegsCombat")),
                new BodyPart(BodyPartType.ArmLeft, load("ArmLeftSimple")),
                new BodyPart(BodyPartType.ArmLeftTwoHanded, load("ArmLeftTwoHanded")),
                new BodyPart(BodyPartType.ArmLeftFist, load("ArmLeftFist")),
                new BodyPart(BodyPartType.ArmRightSimple, load("ArmRightSimple")),
                new BodyPart(BodyPartType.ArmRightTwoHanded, load("ArmRightTwoHanded"))
            });
        }

        private void LoadHumanParts(ContentManager content)
        {
            const string PATH_TO_HUMAN_PARTS = "Sprites/game-objects/Human/";

            Texture2D load(string name) { return content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + name); }

            _bodyParts.Add("Human", new[]
            {
                new BodyPart(BodyPartType.Head, load("Head")),
                new BodyPart(BodyPartType.Chest, load("Body")),
                new BodyPart(BodyPartType.LegsIdle, load("LegsIdle")),
                new BodyPart(BodyPartType.LegsCombat, load("LegsCombat")),
                new BodyPart(BodyPartType.ArmLeft, load("ArmLeftSimple")),
                new BodyPart(BodyPartType.ArmLeftTwoHanded, load("ArmLeftTwoHanded")),
                new BodyPart(BodyPartType.ArmLeftFist, load("ArmLeftFist")),
                new BodyPart(BodyPartType.ArmRightSimple, load("ArmRightSimple")),
                new BodyPart(BodyPartType.ArmRightTwoHanded, load("ArmRightTwoHanded"))
            });

            LoadHumanOutlinedParts(content);
        }

        private void LoadSpecificAnimalParts(ContentManager content, string animalSid)
        {
            const string PATH_TO_PARTS = "Sprites/game-objects/";

            LoadSpecificAnimalPartsBySpecificPath(content, PATH_TO_PARTS, animalSid);
            LoadSpecificAnimalPartsBySpecificPath(content, PATH_TO_PARTS, animalSid + "/Outlined");
        }

        private void LoadSpecificAnimalPartsBySpecificPath(ContentManager content, string path, string animalSid)
        {
            Texture2D load(string path, string animalSid, string name)
            {
                return content.Load<Texture2D>(Path.Combine(path, animalSid, name));
            }

            _animalParts.Add(animalSid, new[]
            {
                new AnimalPart(AnimalPartType.Head, load(path, animalSid, "head")),
                new AnimalPart(AnimalPartType.Body, load(path, animalSid, "body")),
                new AnimalPart(AnimalPartType.Tail, load(path, animalSid, "tail")),
                new AnimalPart(AnimalPartType.LegCloseFront, load(path, animalSid, "leg-close-front")),
                new AnimalPart(AnimalPartType.LegCloseHind, load(path, animalSid, "leg-close-hind")),
                new AnimalPart(AnimalPartType.LegFarFront, load(path, animalSid, "leg-far-front")),
                new AnimalPart(AnimalPartType.LegFarHind, load(path, animalSid, "leg-far-hind"))
            });
        }

        /// <inheritdoc />
        public IEnumerable<BodyPart> GetBodyParts(string sid)
        {
            if (!_bodyParts.TryGetValue(sid, out var bodyParts))
            {
                Debug.Fail("All equipment must be in storage to visualize.");

                return Array.Empty<BodyPart>();
            }

            return bodyParts;
        }

        /// <inheritdoc />
        public IEnumerable<HandPart> GetHandParts(string sid)
        {
            if (!_handParts.TryGetValue(sid, out var handParts))
            {
                Debug.Fail("All equipment must be in storage to visualize.");

                return Array.Empty<HandPart>();
            }

            return handParts;
        }

        /// <inheritdoc />
        public IEnumerable<HeadPart> GetHeadParts(string sid)
        {
            if (!_headParts.TryGetValue(sid, out var headParts))
            {
                Debug.Fail("All equipment must be in storage to visualize.");

                return Array.Empty<HeadPart>();
            }

            return headParts;
        }

        /// <inheritdoc />
        public IEnumerable<BodyPart> GetHumanParts()
        {
            return _bodyParts["Human"];
        }

        /// <inheritdoc />
        public IEnumerable<BodyPart> GetHumanOutlinedParts()
        {
            return _bodyParts["Human/Outlined"];
        }

        public IEnumerable<AnimalPart> GetAnimalParts(string sid)
        {
            return _animalParts[sid];
        }

        /// <inheritdoc />
        public void LoadContent(ContentManager content)
        {
            LoadHumanParts(content);

            LoadBodyParts(content);

            LoadHandParts(content);

            LoadHeadParts(content);

            LoadAnimalParts(content);
        }
    }
}