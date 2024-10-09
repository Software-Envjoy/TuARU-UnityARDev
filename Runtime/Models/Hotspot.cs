using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="Hotspot" />
    /// </summary>
    [Serializable]
    public class Hotspot
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Radius
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets the Position
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the Title by language
        /// </summary>
        public Dictionary<string, string> Title { get; set; }

        /// <summary>
        /// Gets or sets the Description by language
        /// </summary>
        public Dictionary<string, string> Description { get; set; }

        #endregion

        public VisualElement CreateGUI()
        {
            var root = new VisualElement();
            var header = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            var titleLabel = new Label($"Hotspot - {Id}");
            var deleteButton = new Button();
            deleteButton.AddToClassList("button");
            deleteButton.AddToClassList("only-icon");

            return root;
        }
    }
}
