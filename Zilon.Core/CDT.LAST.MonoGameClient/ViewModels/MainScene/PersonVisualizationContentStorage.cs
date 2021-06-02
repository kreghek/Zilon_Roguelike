﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Base implementation of content storage.
    /// </summary>
    internal sealed class PersonVisualizationContentStorage : IPersonVisualizationContentStorage
    {
        private readonly Dictionary<string, BodyPart[]> _bodyParts;
        private readonly Dictionary<string, HandPart[]> _handParts;

        public PersonVisualizationContentStorage()
        {
            _bodyParts = new Dictionary<string, BodyPart[]>();
            _handParts = new Dictionary<string, HandPart[]>();
        }

        private void LoadBodyParts(ContentManager content)
        {
            const string PATH_TO_CASUAL_CLOTHS_PARTS = "Sprites/game-objects/Equipments/CasualCloths/";
            _bodyParts.Add("cloths", new[]
            {
                new BodyPart(BodyPartType.Chest, content.Load<Texture2D>(PATH_TO_CASUAL_CLOTHS_PARTS + "Body")),
                new BodyPart(BodyPartType.LegsIdle, content.Load<Texture2D>(PATH_TO_CASUAL_CLOTHS_PARTS + "LegsIdle")),
                new BodyPart(BodyPartType.LegsCombat,
                    content.Load<Texture2D>(PATH_TO_CASUAL_CLOTHS_PARTS + "LegsCombat"))
            });

            const string PATH_TO_TRAVELER_CLOTHS_PARTS = "Sprites/game-objects/Equipments/TravellerCloths/";
            _bodyParts.Add("traveler", new[]
            {
                new BodyPart(BodyPartType.Chest, content.Load<Texture2D>(PATH_TO_TRAVELER_CLOTHS_PARTS + "Body")),
                new BodyPart(BodyPartType.LegsIdle,
                    content.Load<Texture2D>(PATH_TO_TRAVELER_CLOTHS_PARTS + "LegsIdle")),
                new BodyPart(BodyPartType.LegsCombat,
                    content.Load<Texture2D>(PATH_TO_TRAVELER_CLOTHS_PARTS + "LegsCombat")),
                new BodyPart(BodyPartType.ArmLeft,
                    content.Load<Texture2D>(PATH_TO_TRAVELER_CLOTHS_PARTS + "ArmLeftSimple")),
                new BodyPart(BodyPartType.ArmRightSimple,
                    content.Load<Texture2D>(PATH_TO_TRAVELER_CLOTHS_PARTS + "ArmRightSimple"))
            });
        }

        private void LoadHandParts(ContentManager content)
        {
            const string PATH_TO_HAND_PARTS = "Sprites/game-objects/Equipments/HandEquiped/";

            _handParts.Add("short-sword", new[]
            {
                new HandPart(HandPartType.Base, content.Load<Texture2D>(PATH_TO_HAND_PARTS + "ShortSwordBase"))
            });

            _handParts.Add("wooden-shield", new[]
            {
                new HandPart(HandPartType.Base, content.Load<Texture2D>(PATH_TO_HAND_PARTS + "WoodenShieldBase")),
                new HandPart(HandPartType.BaseBack, content.Load<Texture2D>(PATH_TO_HAND_PARTS + "WoodenShieldBase"))
            });
        }

        private void LoadHumanParts(ContentManager content)
        {
            const string PATH_TO_HUMAN_PARTS = "Sprites/game-objects/Human/";
            _bodyParts.Add("Human", new[]
            {
                new BodyPart(BodyPartType.Head, content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + "Head")),
                new BodyPart(BodyPartType.Chest, content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + "Body")),
                new BodyPart(BodyPartType.LegsIdle, content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + "LegsIdle")),
                new BodyPart(BodyPartType.LegsCombat, content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + "LegsCombat")),
                new BodyPart(BodyPartType.ArmLeft, content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + "ArmLeftSimple")),
                new BodyPart(BodyPartType.ArmRightSimple,
                    content.Load<Texture2D>(PATH_TO_HUMAN_PARTS + "ArmRightSimple"))
            });
        }

        public IEnumerable<BodyPart> GetBodyParts(string sid)
        {
            if (!_bodyParts.TryGetValue(sid, out var bodyParts))
            {
                return Array.Empty<BodyPart>();
            }

            return bodyParts;
        }

        public IEnumerable<HandPart> GetHandParts(string sid)
        {
            if (!_handParts.TryGetValue(sid, out var handParts))
            {
                return Array.Empty<HandPart>();
            }

            return handParts;
        }

        public IEnumerable<BodyPart> GetHumanParts()
        {
            return _bodyParts["Human"];
        }

        public void LoadContent(ContentManager content)
        {
            LoadHumanParts(content);

            LoadBodyParts(content);

            LoadHandParts(content);
        }
    }
}