﻿using System;

namespace StereoKit
{
    /// <summary>A Model is a collection of meshes, materials, and transforms that 
    /// make up a visual element! This is a great way to group together complex 
    /// objects that have multiple parts in them, and in fact, most model formats
    /// are composed this way already!
    /// 
    /// This class contains a number of methods for creation. If you pass in a .obj,
    /// .gltf, or .glb, StereoKit will load that model from file, and assemble materials
    /// and transforms from the file information. But you can also assemble a model
    /// from procedurally generated meshes!
    /// 
    /// Because models include an offset transform for each mesh element, this does have the 
    /// overhead of an extra matrix multiplication in order to execute a render command.
    /// So if you need speed, and only have a single mesh with a precalculated transform
    /// matrix, it can be faster to render a Mesh instead of a Model!</summary>
    public class Model
    {
        internal IntPtr _inst;

        /// <summary>The number of mesh subsets attached to this model.</summary>
        public int SubsetCount => NativeAPI.model_subset_count(_inst);

        /// <summary>This is a bounding box that encapsulates the Model and all its subsets! It's used 
        /// for collision, visibility testing, UI layout, and probably other things. While it's normally
        /// cacluated from the mesh bounds, you can also override this to suit your needs.</summary>
        public Bounds Bounds {
            get => NativeAPI.model_get_bounds(_inst);
            set => NativeAPI.model_set_bounds(_inst, value);
        }

        #region Constructors
        /// <summary>Creates a single mesh subset Model using the indicated Mesh and Material! An
        /// id will be automatically generated for this asset.</summary>
        /// <param name="mesh">Any Mesh asset.</param>
        /// <param name="material">Any Material asset.</param>
        public Model(Mesh mesh, Material material)
        {
            _inst = NativeAPI.model_create_mesh(mesh._inst, material._inst);
        }

        /// <summary>Creates a single mesh subset Model using the indicated Mesh and Material!</summary>
        /// <param name="id">Uses this as the id, so you can Find it later.</param>
        /// <param name="mesh">Any Mesh asset.</param>
        /// <param name="material">Any Material asset.</param>
        public Model(string id, Mesh mesh, Material material)
        {
            _inst = NativeAPI.model_create_mesh(mesh._inst, material._inst);
            if (_inst != IntPtr.Zero)
            {
                NativeAPI.material_set_id(_inst, id);
            }
        }
        private Model(IntPtr model)
        {
            _inst = model;
            if (_inst == IntPtr.Zero)
                Log.Err("Received an empty model!");
        }
        ~Model()
        {
            if (_inst != IntPtr.Zero)
                NativeAPI.model_release(_inst);
        }
        #endregion

        #region Methods
        /// <summary>Gets a link to the Material asset used by the mesh subset!</summary>
        /// <param name="subsetIndex">Index of the mesh subset to get the material for, should be less than SubsetCount.</param>
        /// <returns>A link to the Material asset used by the mesh subset at subsetIndex</returns>
        public Material GetMaterial(int subsetIndex) => new Material(NativeAPI.model_get_material(_inst, subsetIndex));

        /// <summary>Adds this Model to the render queue for this frame! If the Hierarchy has a transform on it,
        /// that transform is combined with the Matrix provided here.</summary>
        /// <param name="transform">A Matrix that will transform the Model from Model Space into the current
        /// Hierarchy Space.</param>
        /// <param name="color">A per-instance color value to pass into the shader! Normally this gets used 
        /// like a material tint. If you're adventurous and don't need per-instance colors, this is a great 
        /// spot to pack in extra per-instance data for the shader!</param>
        public void Draw(Matrix transform, Color color)
            => NativeAPI.render_add_model(_inst, transform, color);

        /// <summary>Adds this Model to the render queue for this frame! If the Hierarchy has a transform on it,
        /// that transform is combined with the Matrix provided here.</summary>
        /// <param name="transform">A Matrix that will transform the Model from Model Space into the current
        /// Hierarchy Space.</param>
        public void Draw(Matrix transform)
            => NativeAPI.render_add_model(_inst, transform, Color.White);
        #endregion

        /// <summary>Looks for a Model asset that's already loaded, matching the given id!</summary>
        /// <param name="modelId">Which Model are you looking for?</param>
        /// <returns>A link to the Model matching 'modelId', null if none is found.</returns>
        public static Model Find(string modelId)
        {
            IntPtr model = NativeAPI.model_find(modelId);
            return model == IntPtr.Zero ? null : new Model(model);
        }

        /// <summary>Loads a list of mesh and material subsets from a .obj, .gltf, or .glb file.</summary>
        /// <param name="file">Name of the file to load! This gets prefixed with the StereoKit asset
        /// folder if no drive letter is specified in the path.</param>
        /// <param name="shader">The shader to use for the model's materials! If null, this will
        /// automatically determine the best shader available to use.</param>
        /// <returns>A Model created from the file, or null if the file failed to load!</returns>
        public static Model FromFile(string file, Shader shader = null)
        {
            IntPtr inst = NativeAPI.model_create_file(file, shader == null ? IntPtr.Zero : shader._inst);
            return inst == IntPtr.Zero ? null : new Model(inst);
        }

        /// <summary>Creates a single mesh subset Model using the indicated Mesh and Material! An
        /// id will be automatically generated for this asset.</summary>
        /// <param name="mesh">Any Mesh asset.</param>
        /// <param name="material">Any Material asset.</param>
        /// <returns>A Model composed of a single mesh and Material.</returns>
        public static Model FromMesh(Mesh mesh, Material material)
        {
            IntPtr inst = NativeAPI.model_create_mesh(mesh._inst, material._inst);
            return inst == IntPtr.Zero ? null : new Model(inst);
        }

        /// <summary>Creates a single mesh subset Model using the indicated Mesh and Material!</summary>
        /// <param name="id">Uses this as the id, so you can Find it later.</param>
        /// <param name="mesh">Any Mesh asset.</param>
        /// <param name="material">Any Material asset.</param>
        /// <returns>A Model composed of a single mesh and Material.</returns>
        public static Model FromMesh(string id, Mesh mesh, Material material)
        {
            IntPtr inst = NativeAPI.model_create_mesh(mesh._inst, material._inst);
            if (inst != IntPtr.Zero)
            {
                NativeAPI.material_set_id(inst, id);
                return new Model(inst);
            }
            return null;
        }
    }
}
