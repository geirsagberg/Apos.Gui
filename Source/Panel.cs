﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Optional;

namespace Apos.Gui {
    /// <summary>
    /// Goal: Container that can hold Components.
    /// </summary>
    public class Panel : Component {

        // Group: Constructors

        public Panel() : this(new Layout()) {
        }
        public Panel(Layout l) {
            Layout = l;
        }

        // Group: Public Variables

        public Point Offset {
            get;
            set;
        } = new Point(0, 0);
        public Size2 Size {
            get;
            set;
        } = new Size2(0, 0);
        public Layout Layout {
            get => _layout;
            set {
                _layout = value;
                _layout.Panel = this;
            }
        }
        public override int PrefWidth => (int)Size.Width;
        public override int PrefHeight => (int)Size.Height;

        // Group: Public Functions

        public virtual void Add(Component e) {
            _children.Add(e);
            e.Parent = Option.Some((Component)this);
        }
        public virtual void Remove(Component e) {
            _children.Remove(e);
            e.Parent = Option.None<Component>();
        }
        public override Component GetPrev(Component c) {
            int index = _children.IndexOf(c) - 1;
            if (index >= 0 && _children.Count > 0) {
                return _children[index];
            }
            return Parent.Map(parent => parent.GetPrev(this)).ValueOr(() => {
                if (_children.Count > 0) {
                    return _children.Last();
                }
                return this;
            });
        }
        public override Component GetNext(Component c) {
            int index = _children.IndexOf(c) + 1;
            if (index < _children.Count) {
                return _children[index];
            }
            return Parent.Map(parent => parent.GetNext(this)).ValueOr(() => {
                if (_children.Count > 0) {
                    return _children[0];
                }
                return this;
            });
        }
        public override Component GetFinal() {
            if (_children.Count > 0) {
                return _children.First();
            }
            return this;
        }
        public override Component GetFinalInverse() {
            if (_children.Count > 0) {
                return _children.Last();
            }
            return this;
        }
        public override void UpdateSetup() {
            base.UpdateSetup();
            if (Layout != null) {
                Layout.RecomputeChildren(_children);
            }
            foreach (Component e in _children) {
                e.UpdateSetup();
            }
        }
        public override bool UpdateInput() {
            bool isUsed = false;

            for (int i = _children.Count - 1; i >= 0; i--) {
                if (_children[i].UpdateInput()) {
                    isUsed = true;
                    break;
                }
            }
            if (!isUsed) {
                isUsed = base.UpdateInput();
            }
            return isUsed;
        }
        public override void Update() {
            base.Update();
            foreach (Component e in _children) {
                e.Update();
            }
        }
        public override void Draw() {
            foreach (Component e in _children) {
                e.Draw();
            }
        }

        // Group: Private Variables

        protected List<Component> _children = new List<Component>();
        protected Layout _layout;
    }
}