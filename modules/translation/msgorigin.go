/*
 * Copyright 2022 VMware, Inc.
 * SPDX-License-Identifier: EPL-2.0
 */

package translation

import (
	"context"
)

// MessageOrigin ...
type MessageOrigin interface {
	GetBundleInfo(context.Context) (*BundleInfo, error)

	GetBundle(context.Context, *BundleID) (*Bundle, error)

	PutBundle(context.Context, *Bundle) error

	// DeleteBundle(context.Context, *BundleID) error
}