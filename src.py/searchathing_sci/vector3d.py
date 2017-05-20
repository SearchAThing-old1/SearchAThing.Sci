"""
* SearchAThing.Sci, Copyright(C) 2015-2017 Lorenzo Delana, License under MIT
*
* The MIT License(MIT)
* Copyright(c) 2015-2017 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.

"""

import sys
import numpy
import math
from searchathing_core.number import *
import locale
from enum import Enum

locale.setlocale(locale.LC_ALL, 'en')


class OrdIdx(Enum):
    X = 0
    Y = 1
    Z = 1


class Vector3D:
    def __init__(self, x: float, y: float, z: float):
        self._x = x
        self._y = y
        self._z = z

    @staticmethod
    def from_array(ar: float) -> 'Vector3D':
        """ build a vector (x,y,0) or (x,y,z) from given 2 or 3 doubles """
        if len(ar) == 3:
            return Vector3D(ar[0], ar[1], ar[2])
        elif len(ar) == 2:
            return Vector3D(ar[0], ar[1], 0)
        else:
            raise Exception("invalid vector3d array argument: must 2 or 3 length")

    @property
    def x(self):
        return self._x

    @property
    def y(self):
        return self._y

    @property
    def z(self):
        return self._z

    @staticmethod
    def zero() -> 'Vector3D':
        return _zero

    @staticmethod
    def xaxis() -> 'Vector3D':
        return _xaxis

    @staticmethod
    def yaxis() -> 'Vector3D':
        return _yaxis

    @staticmethod
    def zaxis() -> 'Vector3D':
        return _zaxis

    def __getitem__(self, index: int) -> float:
        """ indexed vector component """
        if index == 0:
            return self.x
        if index == 1:
            return self.y
        if index == 2:
            return self.z

    def __neg__(self) -> 'Vector3D':
        """ negate """
        return -1.0 * self

    def __add__(self, other: 'Vector3D') -> 'Vector3D':
        """ sum """
        return Vector3D(self.x + other.x, self.y + other.y, self.z + other.z)

    def __sub__(self, other: 'Vector3D') -> 'Vector3D':
        """ sub """
        return Vector3D(self.x - other.x, self.y - other.y, self.z - other.z)

    def __mul__(self, other: float) -> 'Vector3D':
        """ scalar mul """
        return Vector3D(self.x * other, self.y * other, self.z * other)

    def __rmul__(self, other: float) -> 'Vector3D':
        """ scalar mul """
        return self * other

    def __truediv__(self, other: float) -> 'Vector3D':
        """ scalar div """
        return Vector3D(self.x / other, self.y / other, self.z / other)

    def __str__(self):
        """ array string representation """
        return "({0}, {1}, {2})".format(self.x, self.y, self.z)

    @property
    def length(self) -> float:
        return math.sqrt(self.x ** 2 + self.y ** 2 + self.z ** 2)

    def distance(self, other: 'Vector3D') -> float:
        return (self - other).length

    def equals_tol(self, tol: float, x: float, y: float, z: float) -> bool:
        return \
            equals_tol(tol, self.x, x) and \
            equals_tol(tol, self.y, y) and \
            equals_tol(tol, self.z, z)

    def equals_tol2(self, tol: float, other: 'Vector3D') -> bool:
        return self.equals_tol(tol, other.x, other.y, other.z)

    def normalized(self) -> 'Vector3D':
        l = self.length
        return Vector3D(self.x / l, self.y / l, self.z / l)

    def distance2d(self, other: 'Vector3D') -> float:
        """ distance between two points ( without considering Z ) """
        return math.sqrt((self.x - other.x) ** 2 + (self.y - other.y) ** 2)

    def dotproduct(self, other: 'Vector3D') -> float:
        """
        Dot product
        a b = |a| |b| cos(alfa)         
        """
        return self.x * other.x + self.y * other.y + self.z * other.z

    def crossproduct(self, other: 'Vector3D') -> 'Vector3D':
        """
        Cross product ( note that resulting vector is not subjected to normalization )
        a x b = |a| |b| sin(alfa) N
        a x b = |  x  y  z |
                | ax ay az |
                | bx by bz |
        https://en.wikipedia.org/wiki/Cross_product
        """
        return Vector3D(self.y * other.z - self.z * other.y,
                        -self.x * other.z + self.z * other.x,
                        self.x * other.y - self.y * other.x)

    def anglerad(self, tol_len: float, to: 'Vector3D') -> float:
        """
        Angle (rad) between this and other given vector.
        Note: tol must be normalized_length_tolerance()
        if comparing normalized vectors
        """
        if self.equals_tol2(tol_len, to):
            return 0

        # dp = |a| |b| cos(alfa)
        dp = self.dotproduct(to)

        # alfa = acos(dp / (|a| |b|))
        L2 = self.length * to.length
        w = dp / L2

        if equals_tol(tol_len, abs(dp), L2):
            if dp * L2 < 0:
                return numpy.pi
            else:
                return 0

        return math.acos(w)

    def project(self, to: 'Vector3D') -> 'Vector3D':
        """
        project this vector to the other given,
        the resulting vector will be colinear the given one         
        """
        if to.length == 0:
            raise Exception('project on null vector')

        # https://en.wikipedia.org/wiki/Vector_projection
        # http://math.oregonstate.edu/bridge/papers/dot+cross.pdf (fig.1)

        a = self.dotproduct(to)

        return (self.dotproduct(to) / to.length) * to.normalized()

    def set(self, ord_idx: OrdIdx, value: float) -> 'Vector3D':
        """ return a copy of this vector with ordinate ( 0:x 1:y 2:z ) changed """
        x = self.x
        y = self.y
        z = self.z

        if ord_idx == OrdIdx.X:
            x = value
        elif ord_idx == OrdIdx.Y:
            y = value
        elif ord_idx == OrdIdx.Z:
            z = value

        return Vector3D(x, y, z)

    def concordant(self, tol: float, other: 'Vector3D') -> bool:
        """ Note: tol must be Constant.NormalizedLengthTolerance if comparing normalized vectors """
        return self.dotproduct(other) > tol

    def angle_toward(self, tol_len: float, to: 'Vector3D', ref_axis: 'Vector3D') -> float:
        """
        Angle (rad) between this going toward the given other vector
        rotating (right-hand-rule) around the given comparing axis
        Note: tol must be Constant.NormalizedLengthTolerance
        if comparing normalized vectors
        """
        c = self.crossproduct(to)

        if c.concordant(tol_len, ref_axis):
            return self.anglerad(tol_len, to)
        else:
            return 2 * numpy.pi - self.anglerad(tol_len, to)

    #def rotate_about_x_axis(self, angle_rad: float) -> 'Vector3D':


_zero = Vector3D(0, 0, 0)
_xaxis = Vector3D(1, 0, 0)
_yaxis = Vector3D(0, 1, 0)
_zaxis = Vector3D(0, 0, 1)
