﻿using UnityEngine;
using System.Collections;

public class Ground : GenericTileLayer {

	/*
     * Overrides the GenericTileLayer method and calls
     * it from the base.
     *
     * Makes me feel a bit dirty and can be cleaned up
     * considerably without the inheritance part.
     */
    public override void Start() {
        base.Start();

        // Meshes where generated in the base, now do ground specific stuff
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        // Load and assign the ground material
        Material mat = Resources.Load("Materials/Grass", typeof(Material)) as Material;
        renderer.material = mat;
    }
}
