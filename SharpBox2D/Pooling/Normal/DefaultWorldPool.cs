/*******************************************************************************
 * Copyright (c) 2013, Daniel Murphy
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 	* Redistributions of source code must retain the above copyright notice,
 * 	  this list of conditions and the following disclaimer.
 * 	* Redistributions in binary form must reproduce the above copyright notice,
 * 	  this list of conditions and the following disclaimer in the documentation
 * 	  and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/
/**
 * Created at 3:26:14 AM Jan 11, 2011
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpBox2D.Collision;
using SharpBox2D.Common;
using SharpBox2D.Dynamics.Contacts;
using SharpBox2D.Pooling.Normal;

namespace SharpBox2D.Pooling
{
/**
 * Provides object pooling for all objects used in the engine. Objects retrieved from here should
 * only be used temporarily, and then pushed back (with the exception of arrays).
 * 
 * @author Daniel Murphy
 */

    public class ContactStack<CType> : MutableStack<Contact> where CType : Contact
    {
        private IWorldPool pool;
        private Func<IWorldPool, CType> _factoryM;

        public ContactStack(IWorldPool pool, Func<IWorldPool, CType> factoryM, int argInitSize) : base(argInitSize)
        {
            this.pool = pool;
            _factoryM = factoryM;
        }

        protected override Contact newInstance()
        {
            return _factoryM(pool);
        }

        protected override Contact[] newArray(int size)
        {
            return new Contact[size];
        }
    }

    public class Vec2Stack : OrderedStack<Vec2>
    {
        public Vec2Stack(int argStackSize, int argContainerSize)
            : base(argStackSize, argContainerSize)
        {
        }

        protected override Vec2 newInstance()
        {
            return new Vec2();
        }
    }

    public class Vec3Stack : OrderedStack<Vec3>
    {
        public Vec3Stack(int argStackSize, int argContainerSize)
            : base(argStackSize, argContainerSize)
        {
        }

        protected override Vec3 newInstance()
        {
            return new Vec3();
        }
    }

    public class Ma22Stack : OrderedStack<Mat22>
    {
        public Ma22Stack(int argStackSize, int argContainerSize)
            : base(argStackSize, argContainerSize)
        {
        }

        protected override Mat22 newInstance()
        {
            return new Mat22();
        }
    }

    public class AABBStack : OrderedStack<AABB>
    {
        public AABBStack(int argStackSize, int argContainerSize)
            : base(argStackSize, argContainerSize)
        {
        }

        protected override AABB newInstance()
        {
            return new AABB();
        }
    }

    public class RotStack : OrderedStack<Rot>
    {
        public RotStack(int argStackSize, int argContainerSize)
            : base(argStackSize, argContainerSize)
        {
        }

        protected override Rot newInstance()
        {
            return new Rot();
        }
    }

    public class Ma33Stack : OrderedStack<Mat33>
    {
        public Ma33Stack(int argStackSize, int argContainerSize)
            : base(argStackSize, argContainerSize)
        {
        }

        protected override Mat33 newInstance()
        {
            return new Mat33();
        }
    }


    public class DefaultWorldPool : IWorldPool
    {

        private OrderedStack<Vec2> vecs;
        private OrderedStack<Vec3> vec3s;
        private OrderedStack<Mat22> mats;
        private OrderedStack<Mat33> mat33s;
        private OrderedStack<AABB> aabbs;
        private OrderedStack<Rot> rots;

        private Dictionary<int, float[]> afloats = new Dictionary<int, float[]>();
        private Dictionary<int, int[]> aints = new Dictionary<int, int[]>();
        private Dictionary<int, Vec2[]> avecs = new Dictionary<int, Vec2[]>();

        private IWorldPool world;

        public DefaultWorldPool()
        {

        }

        private ContactStack<PolygonContact> pcstack;
        private ContactStack<CircleContact> ccstack;
        private ContactStack<PolygonAndCircleContact> cpstack;
        private ContactStack<EdgeAndCircleContact> ecstack;
        private ContactStack<EdgeAndPolygonContact> epstack;
        private ContactStack<ChainAndCircleContact> chcstack;
        private ContactStack<ChainAndPolygonContact> chpstack;

        private Collision.Collision collision;
        private TimeOfImpact toi;
        private Distance dist;

        public DefaultWorldPool(int argSize, int argContainerSize)
        {
            world = this;
            pcstack = new ContactStack<PolygonContact>(world, pool => new PolygonContact(pool),
                Settings.CONTACT_STACK_INIT_SIZE);
            ccstack = new ContactStack<CircleContact>(world, pool => new CircleContact(pool),
                Settings.CONTACT_STACK_INIT_SIZE);
            cpstack = new ContactStack<PolygonAndCircleContact>(world, pool => new PolygonAndCircleContact(pool),
                Settings.CONTACT_STACK_INIT_SIZE);
            ecstack = new ContactStack<EdgeAndCircleContact>(world, pool => new EdgeAndCircleContact(pool),
                Settings.CONTACT_STACK_INIT_SIZE);
            epstack = new ContactStack<EdgeAndPolygonContact>(world, pool => new EdgeAndPolygonContact(pool),
                Settings.CONTACT_STACK_INIT_SIZE);
            chcstack = new ContactStack<ChainAndCircleContact>(world, pool => new ChainAndCircleContact(pool),
                Settings.CONTACT_STACK_INIT_SIZE);
            chpstack = new ContactStack<ChainAndPolygonContact>(world, pool => new ChainAndPolygonContact(pool),
                Settings.CONTACT_STACK_INIT_SIZE);

            vecs = new Vec2Stack(argSize, argContainerSize);
            vec3s = new Vec3Stack(argSize, argContainerSize);
            mats = new Ma22Stack(argSize, argContainerSize);
            aabbs = new AABBStack(argSize, argContainerSize);
            rots = new RotStack(argSize, argContainerSize);
            mat33s = new Ma33Stack(argSize, argContainerSize);

            dist = new Distance();
            collision = new Collision.Collision(this);
            toi = new TimeOfImpact(this);
        }

        public IDynamicStack<Contact> getPolyContactStack()
        {
            return pcstack;
        }

        public IDynamicStack<Contact> getCircleContactStack()
        {
            return ccstack;
        }

        public IDynamicStack<Contact> getPolyCircleContactStack()
        {
            return cpstack;
        }

        public IDynamicStack<Contact> getEdgeCircleContactStack()
        {
            return ecstack;
        }

        public IDynamicStack<Contact> getEdgePolyContactStack()
        {
            return epstack;
        }

        public IDynamicStack<Contact> getChainCircleContactStack()
        {
            return chcstack;
        }

        public IDynamicStack<Contact> getChainPolyContactStack()
        {
            return chpstack;
        }

        public Vec2 popVec2()
        {
            return vecs.pop();
        }

        public Vec2[] popVec2(int argNum)
        {
            return vecs.pop(argNum);
        }

        public void pushVec2(int argNum)
        {
            vecs.push(argNum);
        }

        public Vec3 popVec3()
        {
            return vec3s.pop();
        }

        public Vec3[] popVec3(int argNum)
        {
            return vec3s.pop(argNum);
        }

        public void pushVec3(int argNum)
        {
            vec3s.push(argNum);
        }

        public Mat22 popMat22()
        {
            return mats.pop();
        }

        public Mat22[] popMat22(int argNum)
        {
            return mats.pop(argNum);
        }

        public void pushMat22(int argNum)
        {
            mats.push(argNum);
        }

        public Mat33 popMat33()
        {
            return mat33s.pop();
        }

        public void pushMat33(int argNum)
        {
            mat33s.push(argNum);
        }

        public AABB popAABB()
        {
            return aabbs.pop();
        }

        public AABB[] popAABB(int argNum)
        {
            return aabbs.pop(argNum);
        }

        public void pushAABB(int argNum)
        {
            aabbs.push(argNum);
        }

        public Rot popRot()
        {
            return rots.pop();
        }

        public void pushRot(int num)
        {
            rots.push(num);
        }

        public Collision.Collision getCollision()
        {
            return collision;
        }

        public TimeOfImpact getTimeOfImpact()
        {
            return toi;
        }

        public Distance getDistance()
        {
            return dist;
        }

        public float[] getFloatArray(int argLength)
        {
            if (!afloats.ContainsKey(argLength))
            {
                afloats.Add(argLength, new float[argLength]);
            }

            Debug.Assert(afloats[argLength].Length == argLength, "Array not built with correct length");
            return afloats[argLength];
        }

        public int[] getIntArray(int argLength)
        {
            if (!aints.ContainsKey(argLength))
            {
                aints.Add(argLength, new int[argLength]);
            }

            Debug.Assert(aints[argLength].Length == argLength, "Array not built with correct length");
            return aints[argLength];
        }

        public Vec2[] getVec2Array(int argLength)
        {
            if (!avecs.ContainsKey(argLength))
            {
                Vec2[] ray = new Vec2[argLength];
                for (int i = 0; i < argLength; i++)
                {
                    ray[i] = new Vec2();
                }
                avecs.Add(argLength, ray);
            }

            Debug.Assert(avecs[argLength].Length == argLength, "Array not built with correct length");
            return avecs[argLength];
        }
    }
}
