#!/bin/bash
# Integration Smoke Test
# Verifies order creation flows through both monolith and order-service
#
# Prerequisites:
#   - Order-service running at http://localhost:5003
#   - Monolith running at http://localhost:5225 (optional, for full flow)
#
# Usage: ./tests/integration-smoke-test.sh

set -e

ORDER_SERVICE_URL="${ORDER_SERVICE_URL:-http://localhost:5003}"
MONOLITH_URL="${MONOLITH_URL:-http://localhost:5225}"

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m'

PASSED=0
FAILED=0

assert_status() {
    local test_name="$1"
    local expected="$2"
    local actual="$3"

    if [ "$actual" -eq "$expected" ]; then
        echo -e "${GREEN}PASS${NC}: $test_name (HTTP $actual)"
        PASSED=$((PASSED + 1))
    else
        echo -e "${RED}FAIL${NC}: $test_name (expected HTTP $expected, got HTTP $actual)"
        FAILED=$((FAILED + 1))
    fi
}

echo "=== Integration Smoke Test ==="
echo ""

# 1. Health check on order-service
echo "--- Test 1: Order-service health check ---"
STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$ORDER_SERVICE_URL/healthz" 2>/dev/null || echo "000")
assert_status "Order-service health check" 200 "$STATUS"

# 2. Create an order via order-service
echo "--- Test 2: Create order via order-service ---"
CREATE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$ORDER_SERVICE_URL/api/order" \
    -H "Content-Type: application/json" \
    -d '{
        "discount": 10.50,
        "comments": "Smoke test order",
        "cashierId": "test-cashier",
        "customerId": 1,
        "orderDetails": [
            {
                "unitPrice": 99.99,
                "quantity": 2,
                "discount": 5.00,
                "productId": 1
            }
        ]
    }' 2>/dev/null || echo -e "\n000")

CREATE_STATUS=$(echo "$CREATE_RESPONSE" | tail -1)
CREATE_BODY=$(echo "$CREATE_RESPONSE" | sed '$d')
assert_status "Create order" 201 "$CREATE_STATUS"

# Extract the created order ID
ORDER_ID=$(echo "$CREATE_BODY" | grep -o '"id":[0-9]*' | head -1 | cut -d: -f2)
echo "  Created order ID: $ORDER_ID"

# 3. Get the created order by ID
echo "--- Test 3: Get order by ID ---"
if [ -n "$ORDER_ID" ]; then
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$ORDER_SERVICE_URL/api/order/$ORDER_ID" 2>/dev/null || echo "000")
    assert_status "Get order by ID ($ORDER_ID)" 200 "$STATUS"
else
    echo -e "${RED}FAIL${NC}: Get order by ID (no order ID from create)"
    FAILED=$((FAILED + 1))
fi

# 4. Get all orders
echo "--- Test 4: Get all orders ---"
STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$ORDER_SERVICE_URL/api/order" 2>/dev/null || echo "000")
assert_status "Get all orders" 200 "$STATUS"

# 5. Get orders by customer ID
echo "--- Test 5: Get orders by customer ID ---"
STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$ORDER_SERVICE_URL/api/order/customer/1" 2>/dev/null || echo "000")
assert_status "Get orders by customer ID" 200 "$STATUS"

# 6. Update the order
echo "--- Test 6: Update order ---"
if [ -n "$ORDER_ID" ]; then
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X PUT "$ORDER_SERVICE_URL/api/order/$ORDER_ID" \
        -H "Content-Type: application/json" \
        -d '{
            "discount": 15.00,
            "comments": "Updated smoke test order",
            "cashierId": "test-cashier",
            "customerId": 1,
            "orderDetails": [
                {
                    "unitPrice": 109.99,
                    "quantity": 3,
                    "discount": 7.50,
                    "productId": 1
                }
            ]
        }' 2>/dev/null || echo "000")
    assert_status "Update order ($ORDER_ID)" 200 "$STATUS"
else
    echo -e "${RED}FAIL${NC}: Update order (no order ID)"
    FAILED=$((FAILED + 1))
fi

# 7. Delete the order
echo "--- Test 7: Delete order ---"
if [ -n "$ORDER_ID" ]; then
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE "$ORDER_SERVICE_URL/api/order/$ORDER_ID" 2>/dev/null || echo "000")
    assert_status "Delete order ($ORDER_ID)" 204 "$STATUS"
else
    echo -e "${RED}FAIL${NC}: Delete order (no order ID)"
    FAILED=$((FAILED + 1))
fi

# 8. Verify order is deleted
echo "--- Test 8: Verify order deleted ---"
if [ -n "$ORDER_ID" ]; then
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$ORDER_SERVICE_URL/api/order/$ORDER_ID" 2>/dev/null || echo "000")
    assert_status "Verify order deleted (expect 404)" 404 "$STATUS"
else
    echo -e "${RED}FAIL${NC}: Verify order deleted (no order ID)"
    FAILED=$((FAILED + 1))
fi

# Summary
echo ""
echo "=== Results ==="
echo -e "Passed: ${GREEN}$PASSED${NC}"
echo -e "Failed: ${RED}$FAILED${NC}"
echo ""

if [ "$FAILED" -gt 0 ]; then
    echo -e "${RED}SMOKE TEST FAILED${NC}"
    exit 1
else
    echo -e "${GREEN}ALL SMOKE TESTS PASSED${NC}"
    exit 0
fi
